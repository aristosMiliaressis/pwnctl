cat << EOF >> efs_mount.sh
#!/bin/bash
sudo yum update -y
sudo mkdir -p ${efs_mount_point}
sudo yum -y install amazon-efs-utils
sudo su -c "echo '${file_system_id}:/ ${efs_mount_point} efs _netdev,tls 0 0' >> /etc/fstab"
sudo mount ${efs_mount_point}
df -k
EOF