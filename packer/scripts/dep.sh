#!/bin/bash
#
# Setup the the box. This runs as root

apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list
echo "deb http://download.mono-project.com/repo/debian wheezy-apache24-compat main" | tee -a /etc/apt/sources.list.d/mono-xamarin.list

apt-get -y update

apt-get -y install curl

# You can install anything you need here.
apt-get -y install mono-complete
apt-get -y install apache2
echo "ServerName localhost" | tee /etc/apache2/conf-available/fqdn.conf
a2enconf fqdn
/etc/init.d/apache2 stop
apt-get install -y libapache2-mod-mono libmono-i18n4.0-cil
apt-get install -y mono-apache-server4

wget -O /tmp/candy-web-security.tar.gz https://s3.eu-central-1.amazonaws.com/ar7z1/happydev/candy-web-security-buildoutput-f14276e.tar.gz
mkdir -p /tmp/candy-web-security && tar -xf /tmp/candy-web-security.tar.gz -C /tmp/candy-web-security
rm -rf /var/www/html/
cp -R /tmp/candy-web-security/_PublishedWebsites/CWS/* /var/www/
chown -R www-data:www-data /var/www/
sudo tee /etc/apache2/sites-available/000-default.conf <<EOF
<VirtualHost *:80>
  ServerName cws
  ServerAdmin zinenkoartem@gmail.com
  DocumentRoot /var/www/
  MonoServerPath cws "/usr/bin/mod-mono-server4"
  MonoDebug cws true
  MonoSetEnv cws MONO_IOMAP=all
  MonoApplications cws "/:/var/www/"

  ErrorLog ${APACHE_LOG_DIR}/error.log
  CustomLog ${APACHE_LOG_DIR}/access.log combined

  <Location "/">
    Allow from all
    Order allow,deny
    MonoSetServerAlias cws
    SetHandler mono
    SetOutputFilter DEFLATE
    SetEnvIfNoCase Request_URI "\.(?:gif|jpe?g|png)$" no-gzip dont-vary
  </Location>
  <IfModule mod_deflate.c>
    AddOutputFilterByType DEFLATE text/html text/plain text/xml text/javascript
  </IfModule>
</VirtualHost>
EOF
/etc/init.d/apache2 start
