#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import os
import sys
import re

REGEX_VERSION = "^\d+\.\d+\.\d+(-.+)?$"

class VersionReplacer(object):
	def __init__(self, keyword, excludeKeywords, oldVersion, newVersion, debug = True):
		if not re.match(REGEX_VERSION, oldVersion.decode()):
			raise RuntimeError("old version str is invalid, should like 1.2.3 or 1.2.3-beta-1")
		if not re.match(REGEX_VERSION, newVersion.decode()):
			raise RuntimeError("new version str is invalid, should like 1.2.3 or 1.2.3-beta-1")
		if oldVersion == newVersion:
			raise RuntimeError("old version and new version is same")
		self.keyword = keyword
		self.excludeKeywords = excludeKeywords
		self.oldVersionStr = oldVersion
		self.newVersionStr = newVersion
		self.oldVersionNum = oldVersion.split(b"-")[0] + b".0"
		self.newVersionNum = newVersion.split(b"-")[0] + b".0"
		if self.oldVersionNum == self.newVersionNum and self.oldVersionStr == self.oldVersionNum:
			# 2.2.0 => 2.2.0-beta-1
			# 2.2.0.0 => 2.2.0.0 => 2.2.0-beta-1.0 (invalid)
			raise RuntimeError("replace stable version with it's pre-release version is not allowed")
		self.debug = debug
		if self.debug:
			print("replace version from %s to %s"%(
				self.oldVersionStr.decode(), self.newVersionStr.decode()))

	def isProjectFile(self, filename):
		return filename.endswith(".csproj") or filename == "package.config"

	def replaceVersion(self, content):
		result = b""
		for line in content.split(b"\n"):
			if self.keyword not in line or any(k in line for k in self.excludeKeywords):
				result += line + b"\n"
				continue
			line_replaced = (line
				.replace(self.oldVersionNum, self.newVersionNum)
				.replace(self.oldVersionStr, self.newVersionStr))
			if self.debug:
				print("-", line.decode())
				print("+", line_replaced.decode())
			result += line_replaced + b"\n"
		if result:
			result = result[:-1] # remove tail \n
		return result
	
	def replaceDirectory(self, path):
		for dirpath, _, filenames in os.walk(path):
			for filename in filenames:
				if not self.isProjectFile(filename):
					continue
				filePath = os.path.join(dirpath, filename)
				if self.debug:
					print("replace", filePath)
				with open(filePath, "rb") as f:
					content = f.read()
				result = self.replaceVersion(content)
				with open(filePath, "wb") as f:
					f.write(result)

def main():
	if len(sys.argv) != 3:
		print("please provide old version and new version, like:")
		print("%s 2.2.0 2.2.1"%sys.argv[0])
		return
	keyword = b"ZKWeb"
	excludeKeywords = [b"ZKWeb.Fork"]
	oldVersion = sys.argv[1].encode()
	newVersion = sys.argv[2].encode()
	versionReplacer = VersionReplacer(keyword, excludeKeywords, oldVersion, newVersion)
	versionReplacer.replaceDirectory(".")

if __name__ == "__main__":
	main()
