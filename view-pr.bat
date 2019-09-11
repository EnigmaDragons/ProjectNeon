@echo off
set arg1=%1
git checkout master
git reset HEAD --hard
git pull
git fetch origin pull/%arg1%/head && git checkout FETCH_HEAD
