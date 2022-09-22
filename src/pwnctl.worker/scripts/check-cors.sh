#!/bib/bash

url=$1

corsy.py -u $url | grep " Severity:" > /dev/null && echo "{\"Asset\":\"$url\", \"tags\":{\"cors-misconfig\":\"true\"}}"