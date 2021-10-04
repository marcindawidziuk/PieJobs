#!/bin/bash
cd PieJobs.UI
npm ci
npm run build
cd ../
cd PieJobs.Api
cp -r ../PieJobs.UI/dist .
dotnet run
#to run in background do:
#killall
#dotnet run &> /dev/null &