# pwntainer

pwntainer is a portable pen-testing environment built on top of the [offensive-docker](https://github.com/aaaguirrep/offensive-docker) image, the goal is to add some infrastructure for testing automation and data persistence.

### Data Persistence
For data persistence i created a cli app called aar (i.e AssetArchiver) that reads assets (e.g urls, domain names, ips, etc) from stdin and writes them to an SQLite database, the idea is to chain aar to other security tools using pipes and redirects instead of storing output to text files.

![pwntainer db model](Images/pwntainer-model.PNG)

### API
datasete API

curl/jq based alias API client

the above components provide us with an easy way to perform CRUD operations from the comfort of our shell.

### Automation

cron job based workflows


#### To Do
- [ ] a workflow that monitors for new subdomains and sends discord notifications
- [ ] a workflow that crawls all endpoints found in the last day
- [ ] a workflow that tries to figure out if certain endpoints (i.e js|json|xml) are static and than monitors them for changes and sends discord notifications if changes found.
- [ ] a workflow that runs some nuclei templates on newly found services???