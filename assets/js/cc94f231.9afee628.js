"use strict";(self.webpackChunkwemogy=self.webpackChunkwemogy||[]).push([[302],{3905:function(e,t,n){n.d(t,{Zo:function(){return c},kt:function(){return f}});var r=n(7294);function i(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function a(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){i(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function p(e,t){if(null==e)return{};var n,r,i=function(e,t){if(null==e)return{};var n,r,i={},o=Object.keys(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||(i[n]=e[n]);return i}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(i[n]=e[n])}return i}var s=r.createContext({}),l=function(e){var t=r.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):a(a({},t),e)),n},c=function(e){var t=l(e.components);return r.createElement(s.Provider,{value:t},e.children)},u="mdxType",m={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},g=r.forwardRef((function(e,t){var n=e.components,i=e.mdxType,o=e.originalType,s=e.parentName,c=p(e,["components","mdxType","originalType","parentName"]),u=l(n),g=i,f=u["".concat(s,".").concat(g)]||u[g]||m[g]||o;return n?r.createElement(f,a(a({ref:t},c),{},{components:n})):r.createElement(f,a({ref:t},c))}));function f(e,t){var n=arguments,i=t&&t.mdxType;if("string"==typeof e||i){var o=n.length,a=new Array(o);a[0]=g;var p={};for(var s in t)hasOwnProperty.call(t,s)&&(p[s]=t[s]);p.originalType=e,p[u]="string"==typeof e?e:i,a[1]=p;for(var l=2;l<o;l++)a[l]=n[l];return r.createElement.apply(null,a)}return r.createElement.apply(null,n)}g.displayName="MDXCreateElement"},5750:function(e,t,n){n.r(t),n.d(t,{assets:function(){return s},contentTitle:function(){return a},default:function(){return m},frontMatter:function(){return o},metadata:function(){return p},toc:function(){return l}});var r=n(3117),i=(n(7294),n(3905));const o={},a="Swagger",p={unversionedId:"swagger",id:"swagger",title:"Swagger",description:"The Swagger Service Extensions make it easy to create API docs.",source:"@site/docs-general/swagger.md",sourceDirName:".",slug:"/swagger",permalink:"/swagger",draft:!1,editUrl:"https://github.com/wemogy/libs-aspnet/edit/main/docs-general/swagger.md",tags:[],version:"current",frontMatter:{},sidebar:"tutorialSidebar",previous:{title:"Overview",permalink:"/"}},s={},l=[{value:"Use multiple OpenApi groups",id:"use-multiple-openapi-groups",level:3}],c={toc:l},u="wrapper";function m(e){let{components:t,...n}=e;return(0,i.kt)(u,(0,r.Z)({},c,n,{components:t,mdxType:"MDXLayout"}),(0,i.kt)("h1",{id:"swagger"},"Swagger"),(0,i.kt)("p",null,"The Swagger Service Extensions make it easy to create API docs."),(0,i.kt)("p",null,"First, we want to make sure, that ",(0,i.kt)("a",{parentName:"p",href:"https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/"},"XML documentation comments")," will be used to describe the API, so first make sure, to add the following lines to the ",(0,i.kt)("inlineCode",{parentName:"p"},".csproj")," file."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-xml",metastring:'title=".csproj"',title:'".csproj"'},"<PropertyGroup>\n  <GenerateDocumentationFile>true</GenerateDocumentationFile>\n  <NoWarn>$(NoWarn);1591</NoWarn>\n</PropertyGroup>\n")),(0,i.kt)("p",null,"Now we can register Swagger in the ",(0,i.kt)("inlineCode",{parentName:"p"},"ConfigureServices")," method of the ",(0,i.kt)("inlineCode",{parentName:"p"},"Startup.cs")," file."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Startup.cs"',title:'"Startup.cs"'},'public void ConfigureServices(IServiceCollection services)\n{\n    // ...\n\n    var xmlDocsFilePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");\n    services.AddSwagger("1.0", "My API", "1.0", "Lorem ipsum dolor.", xmlDocsFilePath);\n}\n')),(0,i.kt)("p",null,(0,i.kt)("strong",{parentName:"p"},"Optional:")," If the API uses Authentication, we can select one from the ",(0,i.kt)("inlineCode",{parentName:"p"},"SecuritySchemeDefaults")," enum."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Startup.cs"',title:'"Startup.cs"'},'public void ConfigureServices(IServiceCollection services)\n{\n    // ...\n\n    var xmlDocsFilePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");\n    services.AddSwagger("1.0", "My API", "1.0", "Lorem ipsum dolor.", xmlDocsFilePath, SecuritySchemeDefaults.JWTBearer)\n}\n')),(0,i.kt)("p",null,"As a last step, make sure to include the ",(0,i.kt)("inlineCode",{parentName:"p"},"UseSwagger()")," and ",(0,i.kt)("inlineCode",{parentName:"p"},"UseSwaggerUI()")," methods in the ",(0,i.kt)("inlineCode",{parentName:"p"},"Configure")," method of the ",(0,i.kt)("inlineCode",{parentName:"p"},"Startup.cs")," file."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Startup.cs"',title:'"Startup.cs"'},'public void Configure(IApplicationBuilder app, IWebHostEnvironment env)\n{\n    // ...\n\n    app.UseSwagger();\n    app.UseSwaggerUI(c =>\n    {\n        c.DocumentTitle = "My API";\n        c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Version 1.0");\n        c.RoutePrefix = string.Empty;\n    });\n}\n')),(0,i.kt)("h3",{id:"use-multiple-openapi-groups"},"Use multiple OpenApi groups"),(0,i.kt)("p",null,"If you want to split your APIs into multiple definition groups, you could define a ",(0,i.kt)("inlineCode",{parentName:"p"},"Dictionary<string, OpenApiInfo>")," to do that."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Startup.cs"',title:'"Startup.cs"'},'private Dictionary<string, OpenApiInfo> _openApiGroups;\n\npublic Startup(IConfiguration configuration)\n{\n    _openApiGroups = new Dictionary<string, OpenApiInfo>\n    {\n        { "public", new OpenApiInfo { Version = Configuration["ApiVersion"], Title = $"Public {Configuration["ApiName"]}", Description = $"This is the {Configuration["ApiName"]} public APIs." } },\n        { "admin", new OpenApiInfo { Version = Configuration["ApiVersion"], Title = $"Admin {Configuration["ApiName"]}", Description = $"This is the {Configuration["ApiName"]} admin APIs." } }\n    };\n}\n\npublic void ConfigureServices(IServiceCollection services)\n{\n    // ...\n\n    var xmlDocsFilePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");\n    services.AddSwagger<HeaderOperationFilter>(_openApiGroups, xmlDocsFilePath);\n}\n\npublic void Configure(IApplicationBuilder app, IWebHostEnvironment env)\n{    \n    // ...\n    \n    app.UseDefaultSetup(env, _openApiGroups);\n    \n    // or you could use UseDefaultSwagger method if you don\'t want to use UseDefaultSetup\n    app.UseDefaultSwagger(_openApiGroups);\n}\n')))}m.isMDXComponent=!0}}]);