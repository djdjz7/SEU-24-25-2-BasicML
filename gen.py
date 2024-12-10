import random

def fun(x):
    # err = random.gauss(0, 1)
    err = 0
    return 789 * x **2 - 765 * x + 420 + err


with open('sampledata.txt', 'w') as f:
    for i in range(100):
        x = random.uniform(-10, 10)
        y = fun(x)
        f.write(f'{x}    {y}\n')