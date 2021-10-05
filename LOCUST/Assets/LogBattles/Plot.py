import matplotlib.pyplot as plt
import numpy as np
import random
import os
import re
import csv

#a = [17000, 16560, 16120, 15690, 15260, 14840, 14420, 14000, 13580, 13160, 12740, 12320, 11900, 11480, 11050, 10620, 10190, 9750, 9310, 8870, 8430, 7980, 7530, 7080, 6620, 18150, 17980, 17820, 17670, 17550, 17450, 17370, 17300, 17250, 17210, 17180, 17160, 17150, 17140, 17140, 17140, 17140, 17140]
#b = [17000, 16690, 16400, 16100, 15820, 15540, 15280, 15020, 14770, 14540, 14310, 14100, 13900, 13710, 13530, 13360, 13190, 13040, 12900, 12770, 12640, 12530, 12420, 12320, 12240, 12160, 11540, 10900, 10240, 9580, 8910, 8210, 7500, 6780, 6030, 5270, 4470, 3660, 2840, 2000, 1150, 290, 0]

plt.figure(0)

with open('log.txt') as f:
	lines = f.readlines()
	
	for line in lines:
		items = line.split(" ");
		list = []
		for i in range(1,len(items)):
			list.append(float(items[i]))
		print(list)
		print("\n")
		plt.plot(range(0,len(list)), list, label=items[0])

plt.xlabel("t")
plt.ylabel("size")

plt.ylim([0,None])

plt.legend()

plt.show()