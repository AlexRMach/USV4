using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using MigraDoc.DocumentObjectModel.Internals;
using System.Data;
using System.Windows.Documents;

namespace ush4.Instruments
{
    public static class FFTCalculation
    {
        static int maxPrimeFactor = 37;
        static int maxPrimeFactorDiv2 = (maxPrimeFactor + 1) / 2;
        static int maxFactorCount = 20;

        static double c3_1 = -1.5000000000000E+00;  /*  c3_1 = cos(2*pi/3)-1;          */
        static double c3_2 = 8.6602540378444E-01;  /*  c3_2 = sin(2*pi/3);            */

        static double u5 = 1.2566370614359E+00;  /*  u5   = 2*pi/5;                 */
        static double c5_1 = -1.2500000000000E+00;  /*  c5_1 = (cos(u5)+cos(2*u5))/2-1;*/
        static double c5_2 = 5.5901699437495E-01;  /*  c5_2 = (cos(u5)-cos(2*u5))/2;  */
        static double c5_3 = -9.5105651629515E-01;  /*  c5_3 = -sin(u5);               */
        static double c5_4 = -1.5388417685876E+00;  /*  c5_4 = -(sin(u5)+sin(2*u5));   */
        static double c5_5 = 3.6327126400268E-01;  /*  c5_5 = (sin(u5)-sin(2*u5));    */
        static double c8 = 7.0710678118655E-01;  /*  c8 = 1/sqrt(2);    */

        static int groupOffset, dataOffset, blockOffset, adr;
        static int groupNo, dataNo, blockNo, twNo;
        static double omega, tw_re, tw_im;

        static double[] twiddleRe = new double[maxPrimeFactor];
        static double[] twiddleIm = new double[maxPrimeFactor];
        static double[] trigRe = new double[maxPrimeFactor];
        static double[] trigIm = new double[maxPrimeFactor];
        static double[] zRe = new double[maxPrimeFactor];
        static double[] zIm = new double[maxPrimeFactor];

        static double[] vRe = new double[maxPrimeFactorDiv2];
        static double[] vIm = new double[maxPrimeFactorDiv2];
        static double[] wRe = new double[maxPrimeFactorDiv2];
        static double[] wIm = new double[maxPrimeFactorDiv2];

        public static Double[] FFT_Magnitude<T>(T[] data, Func<T, Double> RealFunc, int length = 0, int offset = 0)
        {
            int l = length;
            if (length == 0)
                l = data.Count();

            Complex[] c_data = new Complex[l];

            for (int i = 0; i < l; i++)
            {
                c_data[i] = new Complex(RealFunc(data[i + offset]), 0);
            }

            Fourier.Forward(c_data, FourierOptions.NoScaling);

            var result = c_data
                  .Select(x => x.Magnitude)
                  .Take(l).ToArray();

            return result;
        }

        public static Double[] DFT_Magnitude<T>(T[] data, Func<T, Double> RealFunc, int length = 0, int offset = 0)
        {
            int l = length;
            if (length == 0)
                l = data.Count();

            Complex[] c_data = new Complex[l];

            Complex[] res_data = new Complex[l];

            for (int i = 0; i < l; i++)
            {
                c_data[i] = new Complex(RealFunc(data[i + offset]), 0);
            }

            //Fourier.Forward(c_data, FourierOptions.NoScaling);

            for (int k = 0; k < l; k++)
            {
                for (int n = 0; n < l; n++)
                {
                    double arg = -2 * Math.PI * k * n / l;
                    var complex = new Complex(Math.Cos(arg), Math.Sin(arg));
                    res_data[k] += c_data[n] * complex;
                }
            }

            var result = res_data
                  .Select(x => x.Magnitude)
                  .Take(l).ToArray();

            return result;
        }

        unsafe public static Double[] MFT_Magnitude<T>(T[] data, Func<T, Double> RealFunc, int length = 0, int offset = 0)
        {
            Complex im1;

            int i;

            Complex Fk, Hk, Fn, Hn,
                            Zk, Zn,
                            Gk, Gn,
                            W, Wk, Wn;

            int N = length;
            if (length == 0)
                N = data.Count();

            double[] in_data = new double[N];
            double[] out_data = new double[N];

            double[] ampl = new double[N];
            double[] phase = new double[N];

            //  Формируем массив комплексных чисел из входных данных
            for (i = 0; i < N; i++)
            {
                in_data[i] = (RealFunc(data[i + offset]));
            }

            double_splitevenodd(ref in_data, N, 0);

            double[] inRe = new double[N / 2];
            double[] inIm = new double[N / 2];

            for (i = 0; i < N / 2; i++)
            {
                inRe[i] = in_data[i];
                inIm[i] = in_data[i + N / 2];
            }

            fft(N / 2, inRe, inIm, ref ampl, ref phase);

            //double_splitevenodd(in, N, 1);

            W = new Complex(Math.Cos(2 * Math.PI / N), -Math.Sin(2 * Math.PI / N));
            Wk = W;
            Wn = new Complex(Math.Cos(2 * Math.PI * (N / 2 - 1) / N), -Math.Sin(2 * Math.PI * (N / 2 - 1) / N));

            im1 = new Complex(0, 1);

            for (i = 1; i < N / 4 + 1; i++)
            {
                Zk = new Complex(ampl[i], phase[i]);

                Zn = new Complex(ampl[N / 2 - i], phase[N / 2 - i]);

                Fk = (Zk + Complex.Conjugate(Zn)) / 2;

                Hk = (Zk - Complex.Conjugate(Zn)) * (-im1) / 2;

                Fn = (Zn + Complex.Conjugate(Zk)) / 2;

                Hn = (Zn - Complex.Conjugate(Zk)) * (-im1) / 2;

                Gk = Fk + Wk * Hk;

                Gn = Fn + Wn * Hn;

                ampl[i] = Complex.Abs(Gk) * 2 / N;
                //ampl[i] = Gk.Magnitude * 2 / N;

                phase[i] = Gk.Phase;

                ampl[N / 2 - i] = Complex.Abs(Gn) * 2 / N;
                //ampl[N / 2 - i] = Gn.Magnitude * 2 / N;

                phase[N / 2 - i] = Gn.Phase;

                Wk = Wk * W;

                Wn = Wn / W;
            }

            ampl[N / 2] = ampl[0] - phase[0];

            phase[N / 2] = 0;

            if (ampl[N / 2] < 0)
            {
                ampl[N / 2] = -ampl[N / 2];
                phase[N / 2] = Math.PI;
            }

            ampl[N / 2] = ampl[0] + phase[0];

            phase[0] = 0;

            if (ampl[0] < 0)
            {
                ampl[0] = -ampl[0];
                phase[0] = Math.PI;
            }


            var result = ampl.Take(N).ToArray();
            /*
            var result = out_data
                  .Select(x => x.Magnitude)
                  .Take(N).ToArray();*/

            return result;
        }

        static int fft(int n, double[] xRe, double[] xIm, ref double[] yRe, ref double[] yIm)
        {
            int[] sofarRadix = new int[maxFactorCount];
            int[] actualRadix = new int[maxFactorCount];
            int[] remainRadix = new int[maxFactorCount];
            int nFactor = 1;
            int count;


            switch (transTableSetup(sofarRadix, actualRadix, remainRadix, ref nFactor, ref n))
            {
                case 1:// too many factors
                    return 1;

                case 2:// too large prime factor
                    return 2;

                default:
                    break;
            }

            permute(n, nFactor, actualRadix, remainRadix, xRe, xIm, ref yRe, ref yIm);

            for (count = 1; count <= nFactor; count++)
            {
                twiddleTransf(sofarRadix[count], actualRadix[count], remainRadix[count], ref yRe, ref yIm);
            }

            return 0;
        }   /* fft */

        static void permute(int nPoint, int nFact, int[] fact, int[] remain, double[] xRe, double[] xIm, ref double[] yRe, ref double[] yIm)
        {
            int i, j, k;
            int[] count = new int[maxFactorCount];

            for (i = 1; i <= nFact; i++) count[i] = 0;

            k = 0;

            for (i = 0; i <= nPoint - 2; i++)
            {
                yRe[i] = xRe[k];
                yIm[i] = xIm[k];
                j = 1;
                k = k + remain[j];
                count[1] = count[1] + 1;
                while (count[j] >= fact[j])
                {
                    count[j] = 0;
                    k = k - remain[j - 1] + remain[j + 1];
                    j = j + 1;
                    count[j] = count[j] + 1;
                }
            }

            yRe[nPoint - 1] = xRe[nPoint - 1];
            yIm[nPoint - 1] = xIm[nPoint - 1];
        }

        static int transTableSetup(int[] sofar, int[] actual, int[] remain, ref int nFact, ref int nPoints)
        {
            int i;

            switch (factorize(nPoints, ref nFact, actual))
            {
                case 1:/*too many factors*/
                    return 1;
                case 2:/*too large prime factor*/
                    return 2;
                default:
                    break;
            }
            remain[0] = nPoints;

            sofar[1] = 1;

            remain[1] = nPoints / actual[1];

            for (i = 2; i <= nFact; i++)
            {
                sofar[i] = sofar[i - 1] * actual[i - 1];
                remain[i] = remain[i - 1] / actual[i];
            }
            return 0;
        }   /* transTableSetup */

        static int factorize(int n, ref int nFact, int[] fact)
        {
            int i, j, k;
            int nRadix;
            int[] radices = new int[7];
            int[] factors = new int[maxFactorCount];

            nRadix = 6;
            radices[1] = 2;
            radices[2] = 3;
            radices[3] = 4;
            radices[4] = 5;
            radices[5] = 8;
            radices[6] = 10;

            if (n == 1)
            {
                j = 1;
                factors[1] = 1;
            }
            else j = 0;
            i = nRadix;
            while ((n > 1) && (i > 0))
            {
                if ((n % radices[i]) == 0)
                {
                    n = n / radices[i];
                    j = j + 1;
                    if (j > maxFactorCount) return 1; /*too many factors*/
                    factors[j] = radices[i];
                }
                else i = i - 1;
            }
            if (factors[j] == 2)   /*substitute factors 2*8 with 4*4 */
            {
                i = j - 1;
                while ((i > 0) && (factors[i] != 8)) i--;
                if (i > 0)
                {
                    factors[j] = 4;
                    factors[i] = 4;
                }
            }
            if (n > 1)
            {
                for (k = 2; k < Math.Sqrt(n) + 1; k++)
                    while ((n % k) == 0)
                    {
                        n = n / k;
                        j = j + 1;
                        if (j > maxFactorCount) return 1; /*too many factors*/
                        factors[j] = k;
                    }
                if (n > 1)
                {
                    j = j + 1;
                    if (j > maxFactorCount) return 1; /*too many factors*/
                    factors[j] = n;
                }
            }
            for (i = 1; i <= j; i++)
            {
                fact[i] = factors[j - i + 1];
            }

            nFact = j;

            if (factors[j] > maxPrimeFactor) return 2; /*Prime factor of FFT length too large*/
            return 0;  /*factorize OK*/
        }   /* factorize */

        static void twiddleTransf(int sofarRadix, int radix, int remainRadix, ref double[] yRe, ref double[] yIm)

        {   /* twiddleTransf */
            double cosw, sinw, gem;
            double t1_re, t1_im, t2_re, t2_im, t3_re, t3_im;
            double t4_re, t4_im, t5_re, t5_im;
            double m2_re, m2_im, m3_re, m3_im, m4_re, m4_im;
            double m1_re, m1_im, m5_re, m5_im;
            double s1_re, s1_im, s2_re, s2_im, s3_re, s3_im;
            double s4_re, s4_im, s5_re, s5_im;
            initTrig(radix);

            omega = 2 * Math.PI / (double)(sofarRadix * (double)radix);

            cosw = Math.Cos(omega);

            sinw = -Math.Sin(omega);

            tw_re = 1.0;
            tw_im = 0;
            dataOffset = 0;
            groupOffset = dataOffset;
            adr = groupOffset;

            for (dataNo = 0; dataNo < sofarRadix; dataNo++)
            {
                if (sofarRadix > 1)
                {
                    twiddleRe[0] = 1.0;
                    twiddleIm[0] = 0.0;
                    twiddleRe[1] = tw_re;
                    twiddleIm[1] = tw_im;
                    for (twNo = 2; twNo < radix; twNo++)
                    {
                        twiddleRe[twNo] = tw_re * twiddleRe[twNo - 1]
                                       - tw_im * twiddleIm[twNo - 1];
                        twiddleIm[twNo] = tw_im * twiddleRe[twNo - 1]
                                                                     + tw_re * twiddleIm[twNo - 1];
                    }
                    gem = cosw * tw_re - sinw * tw_im;
                    tw_im = sinw * tw_re + cosw * tw_im;
                    tw_re = gem;
                }
                for (groupNo = 0; groupNo < remainRadix; groupNo++)
                {
                    if ((sofarRadix > 1) && (dataNo > 0))
                    {
                        zRe[0] = yRe[adr];
                        zIm[0] = yIm[adr];
                        blockNo = 1;
                        do
                        {
                            adr = adr + sofarRadix;
                            zRe[blockNo] = twiddleRe[blockNo] * yRe[adr]
                            - twiddleIm[blockNo] * yIm[adr];
                            zIm[blockNo] = twiddleRe[blockNo] * yIm[adr]
                            + twiddleIm[blockNo] * yRe[adr];

                            blockNo++;
                        } while (blockNo < radix);
                    }
                    else
                        for (blockNo = 0; blockNo < radix; blockNo++)
                        {
                            zRe[blockNo] = yRe[adr];
                            zIm[blockNo] = yIm[adr];
                            adr = adr + sofarRadix;
                        }
                    switch (radix)
                    {
                        case 2:
                            gem = zRe[0] + zRe[1];
                            zRe[1] = zRe[0] - zRe[1]; zRe[0] = gem;
                            gem = zIm[0] + zIm[1];
                            zIm[1] = zIm[0] - zIm[1]; zIm[0] = gem;
                            break;
                        case 3:
                            t1_re = zRe[1] + zRe[2]; t1_im = zIm[1] + zIm[2];
                            zRe[0] = zRe[0] + t1_re; zIm[0] = zIm[0] + t1_im;
                            m1_re = c3_1 * t1_re; m1_im = c3_1 * t1_im;
                            m2_re = c3_2 * (zIm[1] - zIm[2]);
                            m2_im = c3_2 * (zRe[2] - zRe[1]);
                            s1_re = zRe[0] + m1_re; s1_im = zIm[0] + m1_im;
                            zRe[1] = s1_re + m2_re; zIm[1] = s1_im + m2_im;
                            zRe[2] = s1_re - m2_re; zIm[2] = s1_im - m2_im;
                            break;
                        case 4:
                            t1_re = zRe[0] + zRe[2]; t1_im = zIm[0] + zIm[2];
                            t2_re = zRe[1] + zRe[3]; t2_im = zIm[1] + zIm[3];

                            m2_re = zRe[0] - zRe[2]; m2_im = zIm[0] - zIm[2];
                            m3_re = zIm[1] - zIm[3]; m3_im = zRe[3] - zRe[1];

                            zRe[0] = t1_re + t2_re; zIm[0] = t1_im + t2_im;
                            zRe[2] = t1_re - t2_re; zIm[2] = t1_im - t2_im;
                            zRe[1] = m2_re + m3_re; zIm[1] = m2_im + m3_im;
                            zRe[3] = m2_re - m3_re; zIm[3] = m2_im - m3_im;
                            break;
                        case 5:
                            t1_re = zRe[1] + zRe[4]; t1_im = zIm[1] + zIm[4];
                            t2_re = zRe[2] + zRe[3]; t2_im = zIm[2] + zIm[3];
                            t3_re = zRe[1] - zRe[4]; t3_im = zIm[1] - zIm[4];
                            t4_re = zRe[3] - zRe[2]; t4_im = zIm[3] - zIm[2];
                            t5_re = t1_re + t2_re; t5_im = t1_im + t2_im;
                            zRe[0] = zRe[0] + t5_re; zIm[0] = zIm[0] + t5_im;
                            m1_re = c5_1 * t5_re; m1_im = c5_1 * t5_im;
                            m2_re = c5_2 * (t1_re - t2_re);
                            m2_im = c5_2 * (t1_im - t2_im);

                            m3_re = -c5_3 * (t3_im + t4_im);
                            m3_im = c5_3 * (t3_re + t4_re);
                            m4_re = -c5_4 * t4_im; m4_im = c5_4 * t4_re;
                            m5_re = -c5_5 * t3_im; m5_im = c5_5 * t3_re;

                            s3_re = m3_re - m4_re; s3_im = m3_im - m4_im;
                            s5_re = m3_re + m5_re; s5_im = m3_im + m5_im;
                            s1_re = zRe[0] + m1_re; s1_im = zIm[0] + m1_im;
                            s2_re = s1_re + m2_re; s2_im = s1_im + m2_im;
                            s4_re = s1_re - m2_re; s4_im = s1_im - m2_im;

                            zRe[1] = s2_re + s3_re; zIm[1] = s2_im + s3_im;
                            zRe[2] = s4_re + s5_re; zIm[2] = s4_im + s5_im;
                            zRe[3] = s4_re - s5_re; zIm[3] = s4_im - s5_im;
                            zRe[4] = s2_re - s3_re; zIm[4] = s2_im - s3_im;
                            break;

                        case 8:
                            fft_8();
                            break;

                        case 10:
                            fft_10();
                            break;

                        default:
                            fft_odd(radix);
                            break;
                    }
                    adr = groupOffset;
                    for (blockNo = 0; blockNo < radix; blockNo++)
                    {
                        yRe[adr] = zRe[blockNo]; yIm[adr] = zIm[blockNo];
                        adr = adr + sofarRadix;
                    }
                    groupOffset = groupOffset + sofarRadix * radix;
                    adr = groupOffset;
                }
                dataOffset = dataOffset + 1;
                groupOffset = dataOffset;
                adr = groupOffset;
            }
        }   /* twiddleTransf */

        static void initTrig(int radix)
        {
            int i;
            double w, xre, xim;

            w = 2 * Math.PI / radix;

            trigRe[0] = 1; trigIm[0] = 0;

            xre = Math.Cos(w);
            xim = -Math.Sin(w);

            trigRe[1] = xre; trigIm[1] = xim;

            for (i = 2; i < radix; i++)
            {
                trigRe[i] = xre * trigRe[i - 1] - xim * trigIm[i - 1];
                trigIm[i] = xim * trigRe[i - 1] + xre * trigIm[i - 1];
            }
        }   /* initTrig */

        static void fft_4(double[] aRe, double[] aIm)
        {
            double t1_re, t1_im, t2_re, t2_im;
            double m2_re, m2_im, m3_re, m3_im;

            t1_re = aRe[0] + aRe[2]; t1_im = aIm[0] + aIm[2];
            t2_re = aRe[1] + aRe[3]; t2_im = aIm[1] + aIm[3];

            m2_re = aRe[0] - aRe[2]; m2_im = aIm[0] - aIm[2];
            m3_re = aIm[1] - aIm[3]; m3_im = aRe[3] - aRe[1];

            aRe[0] = t1_re + t2_re; aIm[0] = t1_im + t2_im;
            aRe[2] = t1_re - t2_re; aIm[2] = t1_im - t2_im;
            aRe[1] = m2_re + m3_re; aIm[1] = m2_im + m3_im;
            aRe[3] = m2_re - m3_re; aIm[3] = m2_im - m3_im;
        }   /* fft_4 */


        static void fft_5(double[] aRe, double[] aIm)
        {
            double t1_re, t1_im, t2_re, t2_im, t3_re, t3_im;
            double t4_re, t4_im, t5_re, t5_im;
            double m2_re, m2_im, m3_re, m3_im, m4_re, m4_im;
            double m1_re, m1_im, m5_re, m5_im;
            double s1_re, s1_im, s2_re, s2_im, s3_re, s3_im;
            double s4_re, s4_im, s5_re, s5_im;

            t1_re = aRe[1] + aRe[4]; t1_im = aIm[1] + aIm[4];
            t2_re = aRe[2] + aRe[3]; t2_im = aIm[2] + aIm[3];
            t3_re = aRe[1] - aRe[4]; t3_im = aIm[1] - aIm[4];
            t4_re = aRe[3] - aRe[2]; t4_im = aIm[3] - aIm[2];
            t5_re = t1_re + t2_re; t5_im = t1_im + t2_im;
            aRe[0] = aRe[0] + t5_re; aIm[0] = aIm[0] + t5_im;
            m1_re = c5_1 * t5_re; m1_im = c5_1 * t5_im;
            m2_re = c5_2 * (t1_re - t2_re); m2_im = c5_2 * (t1_im - t2_im);

            m3_re = -c5_3 * (t3_im + t4_im); m3_im = c5_3 * (t3_re + t4_re);
            m4_re = -c5_4 * t4_im; m4_im = c5_4 * t4_re;
            m5_re = -c5_5 * t3_im; m5_im = c5_5 * t3_re;

            s3_re = m3_re - m4_re; s3_im = m3_im - m4_im;
            s5_re = m3_re + m5_re; s5_im = m3_im + m5_im;
            s1_re = aRe[0] + m1_re; s1_im = aIm[0] + m1_im;
            s2_re = s1_re + m2_re; s2_im = s1_im + m2_im;
            s4_re = s1_re - m2_re; s4_im = s1_im - m2_im;

            aRe[1] = s2_re + s3_re; aIm[1] = s2_im + s3_im;
            aRe[2] = s4_re + s5_re; aIm[2] = s4_im + s5_im;
            aRe[3] = s4_re - s5_re; aIm[3] = s4_im - s5_im;
            aRe[4] = s2_re - s3_re; aIm[4] = s2_im - s3_im;
        }   /* fft_5 */

        static void fft_8()
        {
            double[] aRe = new double[4];
            double[] aIm = new double[4];
            double[] bRe = new double[4];
            double[] bIm = new double[4];
            double gem;

            aRe[0] = zRe[0]; bRe[0] = zRe[1];
            aRe[1] = zRe[2]; bRe[1] = zRe[3];
            aRe[2] = zRe[4]; bRe[2] = zRe[5];
            aRe[3] = zRe[6]; bRe[3] = zRe[7];

            aIm[0] = zIm[0]; bIm[0] = zIm[1];
            aIm[1] = zIm[2]; bIm[1] = zIm[3];
            aIm[2] = zIm[4]; bIm[2] = zIm[5];
            aIm[3] = zIm[6]; bIm[3] = zIm[7];

            fft_4(aRe, aIm); fft_4(bRe, bIm);

            gem = c8 * (bRe[1] + bIm[1]);
            bIm[1] = c8 * (bIm[1] - bRe[1]);
            bRe[1] = gem;
            gem = bIm[2];
            bIm[2] = -bRe[2];
            bRe[2] = gem;
            gem = c8 * (bIm[3] - bRe[3]);
            bIm[3] = -c8 * (bRe[3] + bIm[3]);
            bRe[3] = gem;

            zRe[0] = aRe[0] + bRe[0]; zRe[4] = aRe[0] - bRe[0];
            zRe[1] = aRe[1] + bRe[1]; zRe[5] = aRe[1] - bRe[1];
            zRe[2] = aRe[2] + bRe[2]; zRe[6] = aRe[2] - bRe[2];
            zRe[3] = aRe[3] + bRe[3]; zRe[7] = aRe[3] - bRe[3];

            zIm[0] = aIm[0] + bIm[0]; zIm[4] = aIm[0] - bIm[0];
            zIm[1] = aIm[1] + bIm[1]; zIm[5] = aIm[1] - bIm[1];
            zIm[2] = aIm[2] + bIm[2]; zIm[6] = aIm[2] - bIm[2];
            zIm[3] = aIm[3] + bIm[3]; zIm[7] = aIm[3] - bIm[3];
        }   /* fft_8 */

        static void fft_10()
        {
            double[] aRe = new double[5];
            double[] aIm = new double[5];
            double[] bRe = new double[5];
            double[] bIm = new double[5];

            aRe[0] = zRe[0]; bRe[0] = zRe[5];
            aRe[1] = zRe[2]; bRe[1] = zRe[7];
            aRe[2] = zRe[4]; bRe[2] = zRe[9];
            aRe[3] = zRe[6]; bRe[3] = zRe[1];
            aRe[4] = zRe[8]; bRe[4] = zRe[3];

            aIm[0] = zIm[0]; bIm[0] = zIm[5];
            aIm[1] = zIm[2]; bIm[1] = zIm[7];
            aIm[2] = zIm[4]; bIm[2] = zIm[9];
            aIm[3] = zIm[6]; bIm[3] = zIm[1];
            aIm[4] = zIm[8]; bIm[4] = zIm[3];

            fft_5(aRe, aIm); fft_5(bRe, bIm);

            zRe[0] = aRe[0] + bRe[0]; zRe[5] = aRe[0] - bRe[0];
            zRe[6] = aRe[1] + bRe[1]; zRe[1] = aRe[1] - bRe[1];
            zRe[2] = aRe[2] + bRe[2]; zRe[7] = aRe[2] - bRe[2];
            zRe[8] = aRe[3] + bRe[3]; zRe[3] = aRe[3] - bRe[3];
            zRe[4] = aRe[4] + bRe[4]; zRe[9] = aRe[4] - bRe[4];

            zIm[0] = aIm[0] + bIm[0]; zIm[5] = aIm[0] - bIm[0];
            zIm[6] = aIm[1] + bIm[1]; zIm[1] = aIm[1] - bIm[1];
            zIm[2] = aIm[2] + bIm[2]; zIm[7] = aIm[2] - bIm[2];
            zIm[8] = aIm[3] + bIm[3]; zIm[3] = aIm[3] - bIm[3];
            zIm[4] = aIm[4] + bIm[4]; zIm[9] = aIm[4] - bIm[4];
        }   /* fft_10 */

        static void fft_odd(int radix)
        {
            double rere, reim, imre, imim;
            int i, j, k, n, max;

            n = radix;
            max = (n + 1) / 2;
            for (j = 1; j < max; j++)
            {
                vRe[j] = zRe[j] + zRe[n - j];
                vIm[j] = zIm[j] - zIm[n - j];
                wRe[j] = zRe[j] - zRe[n - j];
                wIm[j] = zIm[j] + zIm[n - j];
            }

            for (j = 1; j < max; j++)
            {
                zRe[j] = zRe[0];
                zIm[j] = zIm[0];
                zRe[n - j] = zRe[0];
                zIm[n - j] = zIm[0];
                k = j;
                for (i = 1; i < max; i++)
                {
                    rere = trigRe[k] * vRe[i];
                    imim = trigIm[k] * vIm[i];
                    reim = trigRe[k] * wIm[i];
                    imre = trigIm[k] * wRe[i];

                    zRe[n - j] += rere + imim;
                    zIm[n - j] += reim - imre;
                    zRe[j] += rere - imim;
                    zIm[j] += reim + imre;

                    k = k + j;
                    if (k >= n) k = k - n;
                }
            }
            for (j = 1; j < max; j++)
            {
                zRe[0] = zRe[0] + vRe[j];
                zIm[0] = zIm[0] + wIm[j];
            }
        }   /* fft_odd */

        /*
        функция разделяет массив на элементы с четным/нечетным номером
        на месте. Четные номера собираются впереди исходного массива,
        нечетные - сзади
        mode = 0 - разделяет четные/нечетные, все остальное собирает
        */

        public static int double_splitevenodd(ref double[] in_array, int length, int mode)
        {
            double swapplace;

            int num;

            //out_array = new double[length];

            if ((length % 2) == 1)
            {
                return 1;
            }

            //double[] c_data = new double[length];

            /*
            for (int i = 0; i < length / 2; i++)
            {
                out_array[i] = in_array[i];
            }
            */

            if (mode == 0)
            {
                //	case 0:
                for (int i = 0; i < length / 2; i++)
                {
                    num = 2 * i;

                    swapplace = in_array[num];

                    for (int j = 0; j < i; j++)
                    {
                        in_array[num] = in_array[num - 1];
                        num--;
                    };

                    in_array[num] = swapplace;
                }
                //	break;
            }
            else
            //   default:
            {
                for (int i = length / 2 - 1; i > 0; i--)
                {
                    num = i;
                    swapplace = in_array[num];
                    for (int j = i; j > 0; j--)
                    {
                        in_array[num] = in_array[num + 1];
                        num++;
                    }
                    in_array[num] = swapplace;
                }

                // break;
            };

            return 0;
        }


        /***********************************************************************
        chkfactorize(int n) only checks n for correct factorization, i.e.
        number of factors not more than maxFactorCount and largest prime factor
        less than or equal to maxPrimeFactor
        ************************************************************************/
        static int chkfactorize(int n)
        {
            int i, j, k;
            int nRadix;
            int[] radices = new int[7];
            int factors = 0;

            nRadix = 6;
            radices[1] = 2;
            radices[2] = 3;
            radices[3] = 4;
            radices[4] = 5;
            radices[5] = 8;
            radices[6] = 10;

            if (n == 1)
            {
                j = 1;
                factors = 1;
            }
            else j = 0;
            i = nRadix;

            while ((n > 1) && (i > 0))
            {
                if ((n % radices[i]) == 0)
                {
                    n = n / radices[i];
                    j = j + 1;
                    if (j > maxFactorCount) return 1; /*too many factors*/
                    factors = radices[i];
                }
                else i = i - 1;
            }

            if (n > 1)
            {
                for (k = 2; k < Math.Sqrt(n) + 1; k++)
                {
                    while ((n % k) == 0)
                    {
                        n = n / k;
                        j = j + 1;
                        if (j > maxFactorCount) return 1; /*too many factors*/
                        factors = k;
                    }
                }

                if (n > 1)
                {
                    j = j + 1;
                    if (j > maxFactorCount) return 1; /*too many factors*/
                    factors = n;
                }
            }
            if (factors > maxPrimeFactor) return 2; /*Prime factor of FFT length too large*/
            return 0;  /*factorize OK*/
        }   /* chkfactorize */
    }
}
