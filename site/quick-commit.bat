@echo off
set arg1=%1
git add .
git commit -m %arg1%
git pull
git push
