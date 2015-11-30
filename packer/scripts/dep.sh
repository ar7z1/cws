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

echo "Listen 81" | tee -a /etc/apache2/ports.conf

wget -O /tmp/candy-web-security.tar.gz https://s3.eu-central-1.amazonaws.com/ar7z1/happydev/candy-web-security-buildoutput-01c3212.tar.gz
mkdir -p /tmp/candy-web-security && tar -xf /tmp/candy-web-security.tar.gz -C /tmp/candy-web-security
rm -rf /var/www/html/

mkdir -p /var/www/cws/
cp -R /tmp/candy-web-security/_PublishedWebsites/CWS/* /var/www/cws/

mkdir -p /var/www/alert/
tee /var/www/alert/alert.js <<EOF
alert('alert');
EOF

chown -R www-data:www-data /var/www/
tee /etc/apache2/sites-available/000-default.conf <<EOF
<VirtualHost *:80>
  ServerName cws
  ServerAdmin zinenkoartem@gmail.com
  DocumentRoot /var/www/cws/
  MonoServerPath cws "/usr/bin/mod-mono-server4"
  MonoDebug cws true
  MonoSetEnv cws MONO_IOMAP=all
  MonoApplications cws "/:/var/www/cws/"

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

<VirtualHost *:81>
  ServerName cws
  ServerAdmin zinenkoartem@gmail.com
  DocumentRoot /var/www/alert/

  ErrorLog ${APACHE_LOG_DIR}/error.log
  CustomLog ${APACHE_LOG_DIR}/access.log combined

  <Location "/">
    Allow from all
    Order allow,deny
  </Location>
</VirtualHost>
EOF
/etc/init.d/apache2 start
