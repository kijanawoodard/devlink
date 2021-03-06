﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using DevLink.Public.Features.EmailCommunications;
using DevLink.Public.Models;
using Raven.Client;
using Raven.Client.Document;

namespace DevLink.Public
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var container = BuildContainer();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new FeatureViewEngine());

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
		}

		private IContainer BuildContainer()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			builder.Register(x =>
			{
				var store = new DocumentStore {ConnectionStringName = "RavenDB"};
				store.Initialize();
				return store;
			})
			       .As<IDocumentStore>()
			       .SingleInstance();

			builder.Register(x => x.Resolve<IDocumentStore>().OpenSession())
			       .As<IDocumentSession>()
			       .InstancePerLifetimeScope();

			builder.Register(x => new FormsAuth())
			       .As<IAuthentication>()
			       .SingleInstance();

			builder.Register(x => new ConfigurationManagerBasedWebConfiguration())
			       .As<IAppConfiguration>()
			       .SingleInstance();

			builder.Register(c => new HttpIdentity())
			       .As<ILoggedInMember>()
			       .InstancePerLifetimeScope();

			builder.Register(x => new RavenMemberLookup(x.Resolve<IDocumentSession>(), x.Resolve<ILoggedInMember>()))
			       .As<IFindMembers>()
			       .InstancePerLifetimeScope();

			builder.Register(c => new MandrillEmailService(c.Resolve<IAppConfiguration>()))
			       .As<IEmailNotificationService>()
			       .InstancePerLifetimeScope();

			return builder.Build();
		}
	}

	public class ConfigurationManagerBasedWebConfiguration : IAppConfiguration
	{
		public string GoogleAnalyticsId { get { return ConfigurationManager.AppSettings["google.analytics.id"]; } }
		public string GoogleAnalyticsDomain { get { return ConfigurationManager.AppSettings["google.analytics.domain"]; } }
		public string MadrillApiKey { get { return ConfigurationManager.AppSettings["mandrill.apikey"]; } }
		public string MadrillOriginTarget { get { return ConfigurationManager.AppSettings["mandrill.orgin.target"]; } }
	}

	public interface IAppConfiguration
	{
		string MadrillApiKey { get; }
		string MadrillOriginTarget { get; }
	}

	public class FeatureViewEngine : RazorViewEngine
	{
		//This needs to be initialized to the root namespace of your MVC project.
		//Usually, the namespace of your Global.asax's codebehind will do the trick.
		private static readonly string RootNamespace = typeof(MvcApplication).Namespace;

		public FeatureViewEngine()
		{
			ViewLocationFormats = new[]
			{
				"~/Features/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.cshtml"
			};

			PartialViewLocationFormats = new[]
			{
				"~/Features/{0}.cshtml",
				"~/Features/Shared/{0}.cshtml"
			};

			MasterLocationFormats = new[]
			{
				"~/Features/{0}.cshtml"
			};
		}
		private static IEnumerable<string> GetPath(ControllerContext controllerContext, string viewName)
		{
			var paths = new List<string>();

			//TODO: Cache?
			var controllerType = controllerContext.Controller.GetType();
			var controllerName = controllerType.Name.Replace("Controller", string.Empty);
			var featureFolder = "~" + controllerType.Namespace.Replace(RootNamespace, string.Empty).Replace(".", "/");

			//View in the same folder as controller (controller-folder/view.cshtml)
			paths.Add(string.Format("{0}/{1}.cshtml", featureFolder, viewName));

			//View in a view-folder within controller-folder (controller-folder/views/view.cshtml)
			paths.Add(string.Format("{0}/Views/{1}.cshtml", featureFolder, viewName));

			//View in folder with controller name within a view-folder within a controller-folder (controller-folder/views/controller-name/view.cshtml)
			paths.Add(string.Format("{0}/Views/{1}/{2}.cshtml", featureFolder, controllerName, viewName));

			return paths;
		}

		public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var paths = GetPath(controllerContext, viewName);
			var path = paths.FirstOrDefault(p => VirtualPathProvider.FileExists(p));

			if (path != null)
			{
				return new ViewEngineResult(CreateView(controllerContext, path, null), this);
			}
			//Check the usual suspects
			return base.FindView(controllerContext, viewName, masterName, useCache);
		}

		public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var paths = GetPath(controllerContext, partialViewName);
			var path = paths.FirstOrDefault(p => VirtualPathProvider.FileExists(p));

			if (path != null)
			{
				return new ViewEngineResult(CreateView(controllerContext, path, null), this);
			}
			//check the usual suspects
			return base.FindPartialView(controllerContext, partialViewName, useCache);
		}
	}
}