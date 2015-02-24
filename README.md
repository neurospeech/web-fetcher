# Web Fetcher
Fetches given URL contents and delivers the response further.

We wanted to create a CDN Proxy, where we can route static content of many website's through just one CDN without
having to configure many things.

In short, this web-fetcher acts as a proxy for multiple sites as shown below.

For example, static.cdn-domain.com is hosted with web-fetcher.

Then http://static.cdn-domain.com/mydomain.com/images/logo.gif will request URL http://mydomain.com/images/logo.gif and 
serve content with additional caching parameters. 
