#!/usr/bin/env python3
# remove tailing line break in VERSION.txt
import os
ver = open("VERSION.txt", "rb").read().replace(b"\r", b"").replace(b"\n", b"")
open("VERSION.txt", "wb").write(ver)
os.system("hexdump -C VERSION.txt")
