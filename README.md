<h1>Project Information</h1>
<h3>Description</h3>
Map builder is a set of tools to make <a href="https://www.openstreetmap.org/">OpenStreetMap</a>
data easier to use for the purpose of making games.

<h3>Features intigrated</h3>
<ul>
  <li>Four unit tests.</li>
  <li>Uses various lists to store and retrieve map information and a dictionary for tags.</li>
  <li>Map generation is asynchronous and the page doesn't freeze while processing.</li>
  <li>Has three tables: cells, features and feature points. Whenever the database retrieves cell info it includes the features and featurepoints related to it as well.</li>
</ul>
<h1>Setup</h1>
<h3>Nuget Package List</h3>
<h5>WebBuilder.Api</h5>
<ul>
  <li>Microsoft.AspNetCore.OpenApi 8.0.8</li>
  <li>Swashbuckle.AspNetCore 6.4.0</li>
  <li>S2Geometry 1.0.3</li>
</ul>
<h5>WebBuilder.Data</h5>
<ul>
  <li>Microsoft.EntityFrameworkCore.Design 8.0.10</li>
  <li>Microsoft.EntityFrameworkCore.Sqlite 8.0.10</li>
</ul>
<h5>WebBuilder.Tests</h5>
<ul>
  <li>coverlet.collector 6.0.0</li>
  <li>Microsoft.NET.Test.Sdk 17.8.0</li>
  <li>MSTest.TestAdapter 3.1.1</li>
  <li>MSTest.TestFramework 3.1.1</li>
</ul>
<h5>WebBuilder.Web</h5>
<ul>
  <li>Blazor.Extensions.Canvas 1.1.1</li>
</ul>
