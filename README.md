# ci
Continuous integration tools


## virtualbox-ubuntu-docker
How to create a Docker server on Ubuntu VirtualBox VM

### First
1. Download and Install VirtualBox (https://www.virtualbox.org/wiki/Downloads)  

### Steps for installation using a Ubuntu Linux Server VirtualBox Image
2. Download Ubuntu Linux Server VirtualBox Image (https://s3-eu-west-1.amazonaws.com/virtualboxes.org/ubuntu-14.04-server-amd64.ova.torrent)  (username/password): ubuntu/reverse  
3. Start the VM

### Steps for installation from ISO Image
2. Create Machine, resize RAM, Boot from DVD, Set network to bridge on adapter eth0, create 20 GB disk, add disk to machine, mount iso and Start!!
>VBoxManage createvm --name "ubuntu-14.04" --register  
>VBoxManage modifyvm "ubuntu-14.04" --memory 512  --acpi on --boot1 dvd  
>VBoxManage modifyvm "ubuntu-14.04" --ostype Ubuntu  
>VBoxManage createhd --filename ./ubuntu-14.04.vdi --size 10000  
>VBoxManage storagectl "ubuntu-14.04" --name "IDE Controller" --add ide  
>VBoxManage storageattach "ubuntu-14.04" --storagectl "IDE Controller"  --port 0 --device 0 --type hdd --medium ./ubuntu-14.04.vdi  
>VBoxManage storageattach "ubuntu-14.04" --storagectl "IDE Controller"  --port 1 --device 0 --type dvddrive --medium d:\ISO\ubuntu-14.04.5-server-i386.iso      
>VBoxHeadless --startvm "ubuntu-14.04" 

3. Install Ubuntu

### Common Steps
4. Install Docker -> https://docs.docker.com/installation/ubuntulinux/#for-trusty-14-04  
>sudo apt-get update  
>sudo apt-get install -y --no-install-recommends linux-image-extra-$(uname -r) linux-image-extra-virtual  
>sudo apt-get -y install docker-engine  
>sudo docker run hello-world  
>sudo docker -d  

5. Install GIT
>sudo apt-get install git  


#### Credits
1. https://nakkaya.com/2012/08/30/create-manage-virtualBox-vms-from-the-command-line/  
