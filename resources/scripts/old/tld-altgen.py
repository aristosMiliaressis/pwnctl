#!/usr/bin/python3
import sys
from publicsuffix2 import get_tld, fetch, PublicSuffixList

# TODO: try my own PSL with publicsuffix2 vs https://github.com/weppos/publicsuffix-ruby
# try supplying my own PSL, does it include more tlds?

domain=sys.argv[1]

psl_file = fetch()

public_suffix = get_tld(domain, psl_file)
stripped_domain = domain.replace(public_suffix, "")

psl = PublicSuffixList()
[print(f'{stripped_domain}{suffix}') for suffix in psl.tlds]

# import sys; from publicsuffix2 import get_tld, fetch, PublicSuffixList;psl_file = fetch();public_suffix = get_tld(sys.argv[1], psl_file);stripped_domain = sys.argv[1].replace(public_suffix, "");psl = PublicSuffixList();[print(f'{stripped_domain}{suffix}') for suffix in psl.tlds]
