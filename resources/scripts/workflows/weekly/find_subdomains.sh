#!/bin/bash

get-registration-domains | while read domain;
do
    recon $domain
done