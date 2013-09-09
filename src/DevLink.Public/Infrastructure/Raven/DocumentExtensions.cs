﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Connection;

namespace DevLink.Public.Infrastructure.Raven
{
	public static class DocumentExtensions
	{
		/// <summary>
		/// Checks the database to see if a document with the id specified exists.
		/// </summary>
		/// <param name="databaseCommands">The database commands instance for a particular database.</param>
		/// <param name="key">The document key.</param>
		/// <returns>True if the document exists, false otherwise.</returns>
		public static bool DocumentExists(this IDatabaseCommands databaseCommands, string key)
		{
			var metadata = databaseCommands.Head(key);
			return metadata != null;
		}

		/// <summary>
		/// Checks the database to see if a document with the id specified exists.
		/// </summary>
		/// <param name="session">The advanced session instance for a particular database.</param>
		/// <param name="key">The document key.</param>
		/// <returns>True if the document exists, false otherwise.</returns>
		public static bool DocumentExists(this IAdvancedDocumentSessionOperations session, string key)
		{
			return session.DocumentStore.DatabaseCommands.DocumentExists(key);
		}
	}
}