using System;
using System.IO;
using System.Collections.Generic;

using Goldtect.Utilities.Json;

namespace JsonDemo
{
    public static class JsonDemo
    {
        public static void Main(string[] args) 
		{

            JsonDemo.Demo();
        }

        private static void Demo() {

            IJsonWriter writer;
            JsonArray methods;
            JsonArray filters;
            JsonObject filter;
            JsonObject service;
            JsonObject attrs;
            JsonObject account = new JsonObject();
            JsonArray services = new JsonArray();
            JsonObject credentials = new JsonObject();            

            account.Add("id", "BogusAccount");
            account.Add("enabled", true);
            credentials.Add("username", "BogusAccount");
            credentials.Add("password", "guvfvfabgbarbszlcnffjbeqf");
            account.Add("credentials", credentials);
            account.Add("services", services);

            service = new JsonObject();
            services.Add(service);

            service.Add("id", "pop3");
            service.Add("enabled", true);

            filters = new JsonArray();
            service.Add("transport-filters", filters);
            filter = new JsonObject();
            filter.Add("type", "cidr");
            filter.Add("pattern", "127.0.0.0/8");
            filter.Add("action", "accept");
            filters.Add(filter);
            filter = new JsonObject();
            filter.Add("type", "cidr");
            filter.Add("pattern", "192.168.1.0/8");
            filter.Add("action", "accept");
            filters.Add(filter);

            methods = new JsonArray();
            service.Add("authentication-methods", methods);
            methods.Add("CRAM-MD5");
            methods.Add("APOP");

            attrs = new JsonObject();
            service.Add("named-attributes", attrs);
            attrs.Add("debug-session", true);
            attrs.Add("max-connections", 1);
            attrs.Add("force-secure", false);

            service = new JsonObject();
            services.Add(service);

            service.Add("id", "smtp");
            service.Add("enabled", false);

            filters = new JsonArray();
            service.Add("transport-filters", filters);
            filter = new JsonObject();
            filter.Add("type", "literal");
            filter.Add("pattern", "10.36.1.1");
            filter.Add("action", "accept");
            filters.Add(filter);

            methods = new JsonArray();
            service.Add("authentication-methods", methods);
            methods.Add("CRAM-MD5");
            methods.Add("LOGIN");

            attrs = new JsonObject();
            service.Add("named-attributes", attrs);
            attrs.Add("debug-session", true);
            attrs.Add("allow-relay", true);
            attrs.Add("max-connections", 1);
            attrs.Add("routing-priority-adjust", -10);

            writer = new JsonWriter();
            account.Write(writer);
            Console.WriteLine(writer.ToString());

            writer = new IndentedJsonWriter();
            account.Write(writer);
            Console.WriteLine();
            Console.Write(writer.ToString());

            Console.WriteLine();
            JsonDemo.Parse(writer.ToString());
        }

        private static void Parse(string json) {

            JsonObject account;
            JsonObject credentials;
            JsonString id;
            JsonBoolean enabled;

            using(JsonParser parser = new JsonParser(new StringReader(json), true))
                account = parser.ParseObject();

            id = (JsonString)account["id"];
            enabled = (JsonBoolean)account["enabled"];
            credentials = (JsonObject)account["credentials"];

            Console.WriteLine("Account: {0}", id.Value);
            Console.WriteLine("Username: {0}", credentials["username"]);
            Console.WriteLine("Password: {0}", credentials["password"]);
            Console.WriteLine("Services:");

            foreach(JsonObject service in (JsonArray)account["services"]) {
                id = (JsonString)service["id"];
                enabled = (JsonBoolean)service["enabled"];
                Console.WriteLine("\tService: {0}", id.Value);
                Console.WriteLine("\tStatus: {0}", enabled.Value ? "Enabled" : "Disabled");
                Console.WriteLine("\tFilters:");
                foreach(JsonObject filter in (JsonArray)service["transport-filters"]) {
                    foreach(KeyValuePair<string, IJsonType> item in filter)
                        Console.WriteLine("\t\t{0}: {1}", item.Key, item.Value);
                }
                Console.Write("\tAuth-Methods: ");
                foreach(JsonString method in (JsonArray)service["authentication-methods"])
                    Console.Write("{0} ", method.Value);
                Console.WriteLine();
                Console.WriteLine("\tNamed-Attrs:");
                foreach(KeyValuePair<string, IJsonType> item in (JsonObject)service["named-attributes"])
                    Console.WriteLine("\t\t{0}: {1}", item.Key, item.Value);
            }
        }
    }
}
