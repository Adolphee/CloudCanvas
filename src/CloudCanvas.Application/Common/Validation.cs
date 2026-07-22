using CloudCanvas.Application.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Common
{
    public static class Validate
    {
        public static bool StringValue(string paramName, string? paramValue, string message = "")
        {
            if(String.IsNullOrWhiteSpace(paramValue))
            throw new InvalidArgumentException(String.IsNullOrWhiteSpace(message)?$"Argument ({paramName}) detected with Invalid value ''.": message);
            return true;
        }

        public static T Object<T>(T? validationTarget)
        {
            if (validationTarget == null) throw new ArgumentNullException(nameof(validationTarget));
            var context = new ValidationContext(validationTarget);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(validationTarget, context, results, true);
            if (!isValid) throw new InvalidArgumentException($"Argument of type {typeof(T).Name} did not pass validation. Reasons: {results.ToArray().ToString()}");
            return validationTarget;
        }

        /// <summary>
        /// Loads and returns the main JSON schema for CloudCanvas, resolving any referenced subschemas.
        /// </summary>
        /// <remarks>This method reads the primary schema file and a referenced subschema file from the
        /// application's base directory. It resolves the subschema using a preloaded resolver to ensure all schema
        /// dependencies are properly handled.</remarks>
        /// <returns>A <see cref="JSchema"/> object representing the main JSON schema for CloudCanvas, with all references resolved.</returns>
        private static JSchema GetCloudCanvasMainJsonSchema()
        {
            string pathToMainSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "servicebus-message.schema.json");
            string pathToBlobMetaSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "blob-metadata.schema.json");

            string MainSchemaJson = File.ReadAllText(pathToMainSchema);
            string BlobMetaSchemaJson = File.ReadAllText(pathToBlobMetaSchema);

            var resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("blob-metadata.schema.json", UriKind.RelativeOrAbsolute), System.Text.Encoding.UTF8.GetBytes(BlobMetaSchemaJson));

            var schema = JSchema.Parse(MainSchemaJson, resolver);
            return schema;
        }
        public static double Number(string paramName, double paramValue, double max = int.MaxValue, double min = 0)
        {
            if(min > paramValue || paramValue > max) 
                throw new ArgumentOutOfRangeException($"Argument({paramName}): value must be between {min} and {max}. Provided instead: {paramValue}");
            return paramValue;
        }

        public static bool JsonWithSchema(string jsonValidationTarget)
        {
            Validate.StringValue(nameof(jsonValidationTarget), jsonValidationTarget);
            IList<string> errors;
            var schema = GetCloudCanvasMainJsonSchema();
            JObject obj;
            try
            {
                obj = JObject.Parse(jsonValidationTarget);
                var isValid = obj != null ? obj.IsValid(schema, out errors) : false;
                if (!isValid) throw new JSonSchemaValidationException("Invalid Json string provided.");
                return isValid;
            }
            catch (JsonReaderException e)
            {
                throw new JSonSchemaValidationException($"Unable to parse '{nameof(jsonValidationTarget)}' to JObject;", e);
            }
        }
    }
}
