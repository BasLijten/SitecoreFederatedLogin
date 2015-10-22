This solution contains a OWIN based federated login solution for sitecore. It's by no means production ready, but it might be an interesting
solution.

to install:

* add the following node to your connectionstrings.config:  <add name="AuthSessionStoreContext" providerName="System.Data.SqlClient" connectionString="Data Source=.\;Initial Catalog=WSFedTokens;Integrated Security=False;User ID=sa;Password=xxxxx;"/>
* it creates a new database when it's needed, login tokens will be stored in this database
* Create a controller rendering "Login" - Controller: "Auth" - Controller Action: "Index"
* Create a controller rendering "Logout" - Controller: "Auth" - Controller Action: "Logout"
* Create a page in the root called "Login" and place the login rendering on this page. - this page is used to login. It requires this path, because of some pipeline extension
* Create a page in the root called "Logout" and place the Logout rendering on this page. 
* Modify your startup.cs to include your own hostnames. If there is just one site, the pipeline branching is not needed
* If there are any questions: please feel free to contact me.