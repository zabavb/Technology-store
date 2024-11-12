<p align="center">
  <img src="https://img.icons8.com/cotton/100/store--v2.png" alt="Technology Store Logo" width="120" height="120">
</p>

<h1 align="center">Technology Store ASP.NET Application</h1>

<h2>Project Overview</h2>
<p>
  Technology Store is a web application developed using ASP.NET and C#. The project features a microservices architecture with Ocelot as the gateway managing routes between three APIs (OrderAPI, ProductAPI, UserAPI) and the client application. It includes cookie-based authorization and field validation. Data is managed and saved in an MS SQL Server database via Entity Framework Core. This app allows admins and managers to manage products, users, and orders through CRUD operations, while clients can view products, search, add items to their basket, make orders, cancel orders, and view their profile with loging, registrating into system.
</p>

<h2>Project Details</h2>
<ul>
  <li>
    <strong>Languages:</strong>
    <a href="https://learn.microsoft.com/en-us/dotnet/csharp/" target="_blank">
      <img src="https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white" alt="C#">
    </a>
  </li>
  <li>
    <strong>Technologies:</strong>
    <a href="https://dotnet.microsoft.com/" target="_blank">
      <img src="https://img.shields.io/badge/.NET%20Framework-512BD4?style=flat&logo=.net&logoColor=white" alt=".NET Framework">
    </a>
    <a href="https://learn.microsoft.com/en-us/ef/" target="_blank">
      <img src="https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat&logo=.net&logoColor=white" alt="Entity Framework">
    </a>
    <a href="https://ocelot.readthedocs.io/en/latest/introduction/gettingstarted.html" target="_blank">
      <img src="https://img.shields.io/badge/Ocelot%20API-6A4A3C?style=flat&logo=api&logoColor=white" alt="Ocelot API">
    </a>
  </li>
  <li>
    <strong>Security:</strong>
    <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies" target="_blank">
      <img src="https://img.shields.io/badge/Cookie%20Auth-4CAF50?style=flat&logo=lock&logoColor=white" alt="Cookie Authentication">
    </a>
  </li>
  <li>
    <strong>Project Type:</strong>
    <a href="https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0" target="_blank">
      <img src="https://img.shields.io/badge/ASP.NET%20Web%20App-0078D6?style=flat&logo=asp.net&logoColor=white" alt="ASP.NET Web App">
    </a>
  </li>
</ul>

<h2>Development Details</h2>
<p>
  This project was a course assignment at IT Step Computer Academy. It demonstrates CRUD operations for product, user, and order management with role-based access control, integrating MS SQL Server with Entity Framework Core. The project also uses Bootstrap for UI styling and Ocelot for managing API routes.
</p>

<h2>Getting Started</h2>
<p><strong>Note:</strong> This project requires an MS SQL Server setup with proper connection strings.</p>
<p>Follow these steps to set up the project:</p>
<ol>
  <li>
    Clone the repository:
    <pre><code>git clone https://github.com/zabavb/Technology-store.git</code></pre>
  </li>
  <li>Configure your MS SQL Server and update the connection strings in the <code>appsettings.json</code> file.</li>
  <li>Install the required NuGet packages, including <strong>Ocelot</strong>, <strong>Microsoft.EntityFrameworkCore</strong>, and others as needed.</li>
  <li>Open the solution file in Visual Studio, build the project, and run the application.</li>
</ol>

<h2>Features</h2>
<ul>
  <li><strong>Role-based Access:</strong> Admins and managers can manage products, users, and orders, while clients have restricted access to view products, manage their basket, and place/cancel orders.</li>
  <li><strong>Security:</strong> Cookie-based authentication with field validation for secure data handling.</li>
  <li><strong>API Management:</strong> Ocelot handles API routing between client and microservices, with a clean, streamlined gateway setup.</li>
</ul>

<h2>Usage</h2>
<p>
  To use this application, ensure you have the necessary permissions and connection strings configured. Admins and managers can manage products, users, and orders, while clients can explore product catalogs, manage orders, and view their profiles.
</p>

<h2>Contributing</h2>
<p>Contributions are welcome! If you have any suggestions or improvements, feel free to fork the repository and submit a pull request.</p>
<ol>
  <li>Fork the Repository: Click the "Fork" button at the top-right of this page.</li>
  <li>Create a Branch: Create a new branch for your changes.</li>
  <li>Commit Changes: Make your changes and commit them with a descriptive message.</li>
  <li>Push to Your Fork: Push your changes to your forked repository.</li>
  <li>Submit a Pull Request: Go to the "Pull Requests" tab and submit a new pull request.</li>
</ol>

<h2>Contact</h2>
<p>
  For any questions or inquiries, you can reach me at <a href="mailto:bilonizkavik@agmail.com">mail</a> or connect with me on <a href="https://www.linkedin.com/in/viktor-bilonizhka" target="_blank">LinkedIn</a>.
</p>

<h2>References</h2>
<ul>
  <li><a href="https://learn.microsoft.com/en-us/dotnet/csharp/" target="_blank">C# Documentation</a></li>
  <li><a href="https://learn.microsoft.com/en-us/ef/" target="_blank">Entity Framework Documentation</a></li>
  <li><a href="https://ocelot.readthedocs.io/en/latest/introduction/gettingstarted.html" target="_blank">Ocelot API Documentation</a></li>
</ul>

<h2>Acknowledgements</h2>
<ul>
  <li>Thanks to IT Step Academy for providing the resources and guidance for this project.</li>
  <li>Special thanks to Microsoft for their comprehensive documentation and cloud services.</li>
  <li>Gratitude to the open-source community for NuGet packages and contributions.</li>
</ul>

<hr>

<p align="center">
  Feel free to modify or extend this README to fit your needs better. Happy coding!
</p>
