This solution contains a OWIN based federated login solution for sitecore. It's by no means production ready, but it might be an interesting
solution.

to install:

* add the following node to your connectionstrings.config:  
```
<add name="AuthSessionStoreContext" providerName="System.Data.SqlClient" connectionString="Data Source=.\;Initial Catalog=WSFedTokens;Integrated Security=False;User ID=sa;Password=xxxxx;"/>
```
* Database connection notes:
  * it creates a new database when it's needed, login tokens will be stored in this database
  * Note that you should replace the 'Data Source' value with your database server instance. Copy this part from your 'core' database connection string.
* Install controller renderings:
  * Open up Content Editor on the master database and navigate to /sitecore/layout/Renderings 
  * Create a folder named 'Owin'
  * Create a controller rendering "Login" - Controller: "Auth" - Controller Action: "Index"
  * Create a controller rendering "Logout" - Controller: "Auth" - Controller Action: "Logout"
* Create a page in the root of your site called "Login" and place the login rendering on this page.
  * This page is used to login. It requires this path, because of some pipeline extension
  * Ensure you specify a valid placeholder. For example, if using the default Sitecore sample layout, use the placeholder 'main'
* Create a page in the root of your site called "Logout" and place the Logout rendering on this page. 
  * Ensure you specify a valid placeholder. For example, if using the default Sitecore sample layout, use the placeholder 'main'
* Modify your startup.cs to include your own hostnames. If there is just one site, the pipeline branching is not needed
  * **MetadataAddress**: This is where your login metadata resides in your STS provider.
  * **Wtrealm**: This is your domain that will be accepting users and will be requesting tokens
  * **Wreply**: This is the full URL to the login page you created in your site.
* Update Sitecore references to match to your current version of Sitecore
  * Note that references to Tracker.Current.Context do not compile in 8.2 (and some other versions of Sitecore 8). These should be updated to Sitecore.Context.Session to allow the project to compile.
* Deploy the files and compiled code to your website.
* If there are any questions: please feel free to contact me.
