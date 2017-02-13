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
* If there are any questions: please feel free to contact me.
