#!/bin/bash
#
# Setup the the box. This runs as root

apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list
echo "deb http://download.mono-project.com/repo/debian wheezy-apache24-compat main" | tee -a /etc/apt/sources.list.d/mono-xamarin.list

apt-get -y update

apt-get -y install curl

# You can install anything you need here.
apt-get install mono-runtime
apt-get install apache2
/etc/init.d/apache2 stop
apt-get install libapache2-mod-mono libmono-i18n4.0-cil
apt-get install mono-apache-server4
