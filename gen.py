import random

def fun(x):
    err = random.gauss(0, 1)
    return 1198 * x **2 - 742 * x + 966 + err


with open('sampledata.txt', 'w') as f:
    for i in range(1000):
        x = random.uniform(-10, 10)
        y = fun(x)
        f.write(f'{x}    {y}\n')