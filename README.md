Just a web interface to run your bash scripts. Build with .NET, SQLite and Vue.js

Default credentials:
``` 
username: admin
password: password
 ```


# Installation
download and install both .NET 5 sdk and npm
```
cd PieJobs.UI
npm ci
npm run build
cd ../
cd PieJobs.Api
cp -r ../PieJobs.UI/dist .

#to run in background do:
killall PieJobs.Api
dotnet run &> /dev/null &
```
For SSL/reverse proxy it's best to run it through nginx

# Sample script

```
#!/bin/bash -ex
# create temporary folder
cd $(mktemp -d)
git clone https://github.com/marcindawidziuk/marcin-personal-site.git
cd marcin-personal-site

#run your build commands here
npm ci
npm run generate

#copy files
rm -rf /home/admin/public/marcin-site/
mkdir /home/admin/public/marcin-site/
cp -a ./dist/. /home/admin/public/marcin-site/
chmod 0777 -R /home/admin/public/marcin-site/

#remove temporary folder as no longer needed
rm -rf $(pwd)
```


# Screenshots
![firefox_1fTerH8053](https://user-images.githubusercontent.com/11295543/142762021-a33a88ec-cbb7-46b9-ab8a-95f09e95171f.png)
![firefox_aj7EZosuRJ](https://user-images.githubusercontent.com/11295543/142762022-6d877f49-331e-4c93-b11f-0045702f9b5e.png)

![firefox_HbYz4CzV2u](https://user-images.githubusercontent.com/11295543/142762117-8ab0a2cc-aa0f-444b-93cd-3bb81fa0e77d.png)
