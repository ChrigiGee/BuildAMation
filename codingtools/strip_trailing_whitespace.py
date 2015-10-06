#!/usr/bin/python

from convert_line_endings import convert_line_endings
import os
import re
import sys

def strip_trailing_whitespace(file):
    with open(file, mode='rt') as infile:
        lines = infile.readlines()
    with open(file, mode='wt') as outfile:
        for line in lines:
            stripped = re.sub('[ \t]+$', '', line)
            outfile.write(stripped)
    if sys.platform.startswith("win"):
        convert_line_endings(file)

def processPath(path, extensionList):
    if os.path.isfile(path):
        strip_trailing_whitespace(path)
    else:
      for root, dirs, files in os.walk(path):
          for dir in dirs:
              processPath(os.path.join(root, dir), extensionList)
          for file in files:
              fileExt = os.path.splitext(file)[1]
              if fileExt in extensionList:
                  fullPath = os.path.join(root, file)
                  strip_trailing_whitespace(fullPath)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        extensions = sys.argv[2:]
        if not extensions:
            extensions = ['.cs']
        processPath(sys.argv[1], extensions)
    else:
        processPath('.', ['.cs'])
        processPath('tests', ['.h', '.c', '.cpp', '.m', '.mm'])
