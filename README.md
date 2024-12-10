# SEU 24-25-2 Comprehensive Computing Course Project
## Basic Machine Learning Algorithm (Coefficient Prediction)

> [!IMPORTANT]
>
> This is only a personal hoppy project and **DOES NOT**
> and **WILL NEVER** comply with the course standard.
>
> This is not written in C++, and this does not use the
> required iteration algorithm.

This project aims to predict the coefficients of a sqaure
polynominals:

$$
f(x) = ax^2 + bx + c
$$

To run the program, simply
~~~bash
dotnet run
~~~
if you already have sample data in `sampledata.txt`.

### Input Format

The program reads input data from `sampledata.txt` with the
following format:

~~~plain
x1   f(x1)
x2   f(x2)
...
~~~

Example:

~~~plain
-5.000000     91.337667
-4.600000     79.001885
-4.200000     62.493153
-3.800000     54.414173
-3.400000     43.886765
-3.000000     33.492312
-2.600000     26.814408
-2.200000     21.254624
-1.800000     19.370397
-1.400000     14.657437
-1.000000     7.850113
-0.600000     10.762923
-0.200000     8.197404
0.200000     8.368945
0.600000     11.322743
1.000000     13.795034
1.400000     18.483856
1.800000     25.921698
2.200000     32.881034
2.600000     41.145192
3.000000     49.871497
3.400000     58.680513
3.800000     72.509239
4.200000     86.542235
4.600000     99.736894
5.000000     115.834693
5.400000     132.294885
5.800000     149.248559
~~~

Input data can be generated using `gen.py` in this repository.

### Performance
Accuracy is measure by:
$\frac{1}{n}\sum_{i=1}^n (f(x_i) - y_i)^2$ where $f(x_i)$
is the predicted value and $y_i$ is the observed value.

When input data satisfies $\sigma^2 = 0$, this program should
be able to predict the coefficients of the square polynomial with
$\sigma^2 < 10^{-5}$ within ~1000 iterations, if the coefficients
are within $[-1000, 1000]$, with the following configuration:

~~~csharp
const int POP_SIZE = 10000;
const int SUB_POP_COUNT = 10; // doesnt't seem to affect a lot :-(
const double MAX_ERROR = 0.00001;
const int ITERATIONS = 1000;
~~~

And with the example given previously, the program can produce
the following prediction after 1000 iterations:

~~~plain
Iteration 1000: Best E = 1.6307074392476946
Training Finished @ 1000. Best coefficients:
A: 3.7779897797213544
B: 2.443004976848938
C: 8.622806211927859
Training Time: 00:00:02.7257200
~~~