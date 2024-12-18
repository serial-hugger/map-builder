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
<h3>Steps</h3>
<ol>
  <li>Clone the repo and open it.</li>
  <li>Delete the "Migrations" folder from the MapBuilder.Data project if it is there.</li>
  <li>(Optional) If you ran the app previously and want a clean database, delete the file "cells.db" located in "C:\Users\%USER%\AppData\Local" on Windows or "/home/%USER%/.local/share/" on Linux.</li>
  <li>Right click MapBuilder.Data and open it in the terminal.</li>
  <li>Run the command "dotnet ef migrations add InitialCreate".</li>
  <li>Run the command "dotnet ef database update".</li>
  <li>Make MapBuilder.Web the startup item.</li>
  <li>Make http the launch option.</li>
  <li>Run the app.</li>
  <img src=https://github.com/user-attachments/assets/60db73ae-bf9c-4f91-9b20-e60de2987648/>
      <li>You should have a page looking like this open up in a browser, if it does not please make sure the <a href="#NuGet">NuGet package list</a> below are installed and are the correct versions.</li>
</ol>
<h3>Quick testing</h3>
<ol>
  <li>Click "Get Map".</li>
  <li>The generator is already set up to generate a map, you can just click generate to go ahead and generate one.</li>
  <li>Use presets to quickly generate maps of different areas, or enter your own coordinates to generate one of your favorite place.</li>
  <li>Draw settings and feature settings are a very customizable but has presets to make it quicker, I recommend playing with the draw settings to generate maps with different looks.</li>
</ol>
<h3>Troubleshooting</h3>
<ol>
  <li>If there are errors with the dotnet ef commands, the app targets dotnet 8.0 so make sure the tools are installed.</li>
</ol>
<h3 id = "NuGet">Nuget Package List</h3>
<h4>These are the explicit packages installed on each project.</h4>
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
<h5>WebBuilder.Shared</h5>
<ul>
  <li>Newtonsoft.Json 13.0.3</li>
  <li>S2Geometry 1.0.3</li>
</ul>
