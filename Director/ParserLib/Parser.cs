using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Globalization;

namespace Director.ParserLib
{
    public class Parser
    {
        //
        // general syntactical characters
        //
        private const char VARIABLE_CHARACTER = '$';
        private const char MAIN_SYNTAX_CHARACTER = '#';
        private const char ESCAPE_CHARACTER = '\\';

        private const string SOURCE_TEMPLATE = "template";
        private const string SOURCE_RESPONSE = "response";

        //
        // request specific constants
        //
        private const char REQUEST_FN_LEFT_PARENTHESIS = '(';
        private const char REQUEST_FN_RIGHT_PARENTHESIS = ')';
        private const char REQUEST_FN_ARGUMENT_SEPARATOR = ',';

        private const string REQUEST_FN_RAND_INT = "randInt";
        private const string REQUEST_FN_RAND_FLOAT = "randFloat";
        private const string REQUEST_FN_RAND_STRING = "randString";
        private const string REQUEST_FN_SEQUENCE = "sequence";

        private static readonly char[] REQUEST_FN_RAND_STRING_GROUP_SYMBOLS = {
            'a',
            'A',
            '1',
            '!'
        };
        private static readonly string[] REQUEST_FN_RAND_STRING_GROUPS = {
            "abcdefghijklmnopqrstuvwxyz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "0123456789",
            "!@#$%^&*()_+-=[]{};':|,./<>?"
        };

        //
        // response specific constants
        //
        private const char RESPONSE_OPERATION_SEPARATOR = '_';

        private const string RESPONSE_TYPE_STRING = "string";
        private const string RESPONSE_TYPE_INTEGER = "integer";
        private const string RESPONSE_TYPE_FLOAT = "real";
        private const string RESPONSE_TYPE_BOOLEAN = "boolean";
        private const string RESPONSE_TYPE_OBJECT = "object";
        private const string RESPONSE_TYPE_ARRAY = "array";

        private const string RESPONSE_OPERATION_EQUAL = "eq";
        private static readonly string[] RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_BOOLEAN, RESPONSE_TYPE_OBJECT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_NOT_EQUAL = "ne";
        private static readonly string[] RESPONSE_OPERATION_NOT_EQUAL_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_BOOLEAN, RESPONSE_TYPE_OBJECT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_LESS_THAN = "lt";
        private static readonly string[] RESPONSE_OPERATION_LESS_THAN_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_LESS_THAN_OR_EQUAL = "le";
        private static readonly string[] RESPONSE_OPERATION_LESS_THAN_OR_EQUAL_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_GREATER_THAN = "gt";
        private static readonly string[] RESPONSE_OPERATION_GREATER_THAN_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_GREATER_THAN_OR_EQUAL = "ge";
        private static readonly string[] RESPONSE_OPERATION_GREATER_THAN_OR_EQUAL_ALLOWED_TYPES = { RESPONSE_TYPE_STRING, RESPONSE_TYPE_INTEGER, RESPONSE_TYPE_FLOAT, RESPONSE_TYPE_ARRAY };
        private const string RESPONSE_OPERATION_MATCHING_REGEXP_PATTERN = "mp";
        private static readonly string[] RESPONSE_OPERATION_MATCHING_REGEXP_PATTERN_ALLOWED_TYPES = { RESPONSE_TYPE_STRING };

        private const string RESPONSE_OPERATION_MODIFIER_USE_VARIABLE = "uv";
        private const string RESPONSE_OPERATION_MODIFIER_IF_PRESENT = "ip";

        private const string RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_TRUE = "true";
        private const string RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_FALSE = "false";

        //
        // parser error messages
        //
        private static readonly string ERR_MSG_MISSING_VARIABLE_CHARACTER = "Missing closing variable character (" + VARIABLE_CHARACTER + "). Either provide one or escape this one like " + ESCAPE_CHARACTER + VARIABLE_CHARACTER + " .";
        private static readonly string ERR_MSG_MISSING_MAIN_SYNTAX_CHARACTER = "Missing closing function character (" + MAIN_SYNTAX_CHARACTER + "). Either provide one or escape this one like " + ESCAPE_CHARACTER + MAIN_SYNTAX_CHARACTER + " .";
        private static readonly string ERR_MSG_UNKNOWN_CUSTOM_VARIABLE = "Variable \"{0}\" is not defined.";
        private static readonly string ERR_MSG_EXPECTING_CHARACTER = "Expecting \"{0}\".";
        private static readonly string ERR_MSG_INVALID_CHARACTER = "Invalid character \"{0}\".";
        private static readonly string ERR_MSG_MISSING_FN_NAME = "Missing function name.";
        private static readonly string ERR_MSG_UNRECOGNIZED_CHARS_AFTER_FN = "Unrecognized characters after defined function.";
        private static readonly string ERR_MSG_SWAPPED_PARENTHESES = "Possible swapped parentheses - " + REQUEST_FN_LEFT_PARENTHESIS + " must be closed with " + REQUEST_FN_RIGHT_PARENTHESIS + ".";
        private static readonly string ERR_MSG_UNRECOGNIZED_FN = "Unrecognized function.";
        private static readonly string ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS = "Invalid number of arguments.";
        private static readonly string ERR_MSG_ARGUMENT_NOT_INTEGER = "Argument \"{0}\" is not a valid integer.";
        private static readonly string ERR_MSG_ARGUMENT_NOT_FLOAT = "Argument \"{0}\" is not a valid float.";
        private static readonly string ERR_MSG_ARGUMENT_TOO_BIG = "Argument \"{0}\" is too big of a number.";
        private static readonly string ERR_MSG_ARGUMENT_INVALID = "Argument \"{0}\" is invalid.";
        private static readonly string ERR_MSG_ARGUMENTS_WRONG_SIZED = "Argument no. {0} (\"{1}\") must be smaller than argument no. {2} (\"{3}\").";
        private static readonly string ERR_MSG_FN_INSIDE_VAR = "Illegal function definition inside variable name.";
        private static readonly string ERR_MSG_UNKNOWN_OPERATION = "Unknown operation type \"{0}\".";
        private static readonly string ERR_MSG_UNKNOWN_TYPE = "Unknown type \"{0}\".";
        private static readonly string ERR_MSG_MISING_TYPE_FOR_OPERATION = "Missing type for operation. If you want to compare the value, type must be specified.";
        private static readonly string ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE = "Operation \"{0}\" does not support \"{1}\" type.";
        private static readonly string ERR_MSG_VALUE_UNCONVERTABLE_TO_TYPE = "Provided value \"{0}\" couldn't be converted to \"{1}\" type.";
        private static readonly string ERR_MSG_ILLEGAL_KEY_IN_RESPONSE = "Response JSON contains \"{0}\" key, that is not defined in the template.";
        private static readonly string ERR_MSG_MISSING_KEY_IN_RESPONSE = "Response JSON does not contain \"{0}\" key, that is defined as mandatory in the template.";
        private static readonly string ERR_MSG_VALUES_DIFFER_IN_TYPE = "Values for matched key \"{0}\" differ in their type (\"{1}\" and \"{2}\").";
        private static readonly string ERR_MSG_COMPARE_VALUES = "Values \"{0}\" defined in template and \"{1}\" received in response don't match.";
        private static readonly string ERR_MSG_COMPARE_TYPES = "Types \"{0}\" defined in template and \"{1}\" received in response don't match.";
        private static readonly string ERR_MSG_COMPARE_OPERATION = "Operation \"{0} {1} {2}\" returned false.";

        //
        // global variables
        //
        private Random random = new Random();

        public ParserResult generateRequest(string template, Dictionary<string, string> customVariables)
        {
            List<ParserError> errors = new List<ParserError>();
            Dictionary<string, ParserItem> root = null; // top layer of internal parser structures of deserialized JSON template

            // if no object with custom variables was passed, create new one
            if (customVariables == null)
                customVariables = new Dictionary<string, string>();

            // TODO: delete this after testing
            customVariables.Add("ca$u", "5");
            customVariables.Add("ahoj", "nazdarek");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            // try to deserialize JSON template into internal parser structures
            try
            {
                root = deserialize(template);
            }
            catch (JsonException e)
            {
                // an error occured during deserializon => basic syntax of json template is wrong
                errors.Add(createError(e.Message, SOURCE_TEMPLATE));
                // add error to the error list and return empty result
                return new ParserResult(errors, null);
            }

            // parse inner syntax written in value of some tags (replace variables, evaluate functions etc...)
            parseInnerSyntaxOfRequestTemplate(root, customVariables, errors);

            // serialize JSON string back from modified internal parser structures
            string result = serialize(root);

            // return the result along with some potential non-critical errors
            return new ParserResult(errors, result);
        }

        public ParserResult validateResponse(string template, Dictionary<string, string> customVariables)
        {
            List<ParserError> errors = new List<ParserError>();
            Dictionary<string, ParserItem> root = null; // top layer of internal parser structures of deserialized JSON template

            // if no object with custom variables was passed, create new one
            if (customVariables == null)
                customVariables = new Dictionary<string, string>();

            // TODO: delete this after testing
            customVariables.Add("ca$u", "5");
            customVariables.Add("ahoj", "nazdarek");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            // try to deserialize JSON template into internal parser structures
            try
            {
                root = deserialize(template);
            }
            catch (JsonException e)
            {
                // an error occured during deserializon => basic syntax of json template is wrong
                errors.Add(createError(e.Message, SOURCE_TEMPLATE));
                // add error to the error list and return empty result
                return new ParserResult(errors, null);
            }

            // parse inner syntax written in value of some tags in order to be able to compare this template with actual received result
            parseInnerSyntaxOfResponseTemplate(root, customVariables, errors);

            // return all possibly found errors
            return new ParserResult(errors, null);
        }

        public ParserResult parseResponse(string template, string response, Dictionary<string, string> customVariables, bool ignoreUndefinedKeys)
        {
            List<ParserError> errors = new List<ParserError>();
            Dictionary<string, ParserItem> template_root = null; // top layer of internal parser structures of deserialized JSON template
            Dictionary<string, ParserItem> response_root = null; // top layer of internal parser structures of deserialized received JSON

            // if no object with custom variables was passed, create new one
            if (customVariables == null)
                customVariables = new Dictionary<string, string>();

            // TODO: delete this after testing
            customVariables.Add("ca$u", "5");
            customVariables.Add("ahoj", "nazdarek");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            // try to deserialize JSON template into internal parser structures
            try
            {
                template_root = deserialize(template);
            }
            catch (JsonException e)
            {
                // an error occured during deserializon => basic syntax of json template is wrong
                errors.Add(createError(e.Message, SOURCE_TEMPLATE));
                // add error to the error list and return empty result
                return new ParserResult(errors, null);
            }

            // try to deserialize received JSON into internal parser structures
            try
            {
                response_root = deserialize(response);
            }
            catch (JsonException e)
            {
                // an error occured during deserializon => basic syntax of received json is wrong
                errors.Add(createError(e.Message, SOURCE_RESPONSE));
                // add error to the error list and return empty result
                return new ParserResult(errors, null);
            }

            // parse inner syntax written in value of some tags in order to be able to compare this template with actual received result
            parseInnerSyntaxOfResponseTemplate(template_root, customVariables, errors);

            // if everything went smoothly so far, compare received JSON with the rules defined in JSON template
            if (errors.Count == 0)
                compareResponseWithTemplate(template_root, response_root, customVariables, errors, ignoreUndefinedKeys);

            // return all possibly found errors
            return new ParserResult(errors, null);
        }

        private ParserError createError(string errorMessage, string source)
        {
            const string LINE_DEF = ", line ";
            const string POSITION_DEF = ", position ";

            int posLine = errorMessage.IndexOf(LINE_DEF);
            int posPosition = errorMessage.IndexOf(POSITION_DEF);
            if ( (posLine != -1) && (posPosition != -1) )
            {
                int line = Convert.ToInt32(Regex.Match(errorMessage.Substring(posLine, posPosition - posLine), @"\d+").Value);
                int position = Convert.ToInt32(Regex.Match(errorMessage.Substring(posPosition, errorMessage.Length - posPosition), @"\d+").Value);
                return new ParserError(line, position, errorMessage.Substring(0, posPosition - LINE_DEF.Length - 1), source);
            }
            return new ParserError(-1, -1, "\nMessage: " + errorMessage, source);
        }

        private Dictionary<string, object> resursiveDeserialization(string template)
        {
            Dictionary<string, object> result;

            // try to deserialize JSON string
            result = JsonConvert.DeserializeObject<Dictionary<string, object>>(template);

            List<string> keys = new List<string>(result.Keys);
            foreach (string key in keys)
            {
                //Type obj_type = result[key].GetType();
                //Console.WriteLine(obj_type.FullName);

                if (result[key] is JObject)
                {
                    result[key] = resursiveDeserialization(result[key].ToString());
                }
            }

            return result;
        }

        private Dictionary<string, ParserItem> deserialize(string template)
        {
            Dictionary<string, ParserItem> result = new Dictionary<string, ParserItem>();
            JsonTextReader reader = new JsonTextReader(new StringReader(template));
            List<Dictionary<string, ParserItem>> indent_path = new List<Dictionary<string, ParserItem>>();
            string found_key = null;
            List<ParserItem> array = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    found_key = (string) reader.Value;
                }

                if  (  
                        (
                            reader.TokenType == JsonToken.String || 
                            reader.TokenType == JsonToken.Integer ||
                            reader.TokenType == JsonToken.Float ||
                            reader.TokenType == JsonToken.Boolean ||
                            reader.TokenType == JsonToken.Null
                        )
                    &&
                        found_key != null
                    )
                {
                    ParserItem item = new ParserItem(reader.LineNumber, reader.LinePosition, reader.Value);

                    if (array == null)
                    {
                        indent_path.Last().Add(found_key, item);
                        found_key = null;
                    }
                    else
                    {
                        array.Add(item);
                    }
                }


                if (reader.TokenType == JsonToken.StartObject)
                {
                    if (indent_path.Count == 0)
                    {
                        indent_path.Add(result);
                    }
                    else
                    {
                        if (found_key != null)
                        {
                            Dictionary<string, ParserItem> sub_node = new Dictionary<string, ParserItem>(); // create new node for new object
                            ParserItem item = new ParserItem(reader.LineNumber, reader.LinePosition, sub_node);
                            indent_path.Last().Add(found_key, item); // add item to the current level
                            found_key = null;
                            indent_path.Add(sub_node); // add new active saving location to the end of this list
                        }
                    }
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    indent_path.RemoveAt(indent_path.Count - 1); // remove last element
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    array = new List<ParserItem>();
                }

                if (reader.TokenType == JsonToken.EndArray)
                {
                    ParserItem item = new ParserItem(reader.LineNumber, reader.LinePosition, array);
                    indent_path.Last().Add(found_key, item);
                    found_key = null;
                    array = null;
                }
            }

            return result;
        }

        private string serialize(Dictionary<string, ParserItem> root)
        {
            StringBuilder sb = new StringBuilder();
            JsonTextWriter writer = new JsonTextWriter(new StringWriter(sb));

            writer.WriteStartObject();
            foreach (KeyValuePair<string, ParserItem> item in root)
            {
                writer.WritePropertyName(item.Key);
                if (item.Value.value is Dictionary<string, ParserItem>) // object
                {
                    writer.WriteRawValue(serialize((Dictionary<string, ParserItem>)item.Value.value)); // recursive serialization
                }
                else
                {
                    if (item.Value.value is List<ParserItem>) // array
                    {
                        writer.WriteStartArray();
                        List<ParserItem> list = (List<ParserItem>)item.Value.value;
                        foreach (ParserItem list_item in list) // do the same serialization for every item of that list/array
                        {
                            if (list_item.value is Dictionary<string, ParserItem>) // object
                            {
                                writer.WriteRawValue(serialize((Dictionary<string, ParserItem>)list_item.value)); // recursive serialization
                            }
                            else // simple data type
                            {
                                writer.WriteValue(list_item.value);
                            }
                        }
                        writer.WriteEndArray();
                    }
                    else // simple data type
                    {
                        writer.WriteValue(item.Value.value);
                    }
                }
            }
            writer.WriteEndObject();

            return sb.ToString();
        }

        private void parseInnerSyntaxOfRequestTemplate(Dictionary<string, ParserItem> root, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            List<string> keys = new List<string>(root.Keys);
            foreach (string key in keys)
            {

                if (root[key].value is string)
                {
                    // replace variables, generate sequences, random values etc...
                    root[key] = applyFunctions(root[key], customVariables, errors);
                }
                if (root[key].value is Dictionary<string, ParserItem>) // Dictionary type means that original JSON had an object at this position
                {
                    // call the same function recursively for the entire sub-node
                    parseInnerSyntaxOfRequestTemplate((Dictionary<string, ParserItem>)root[key].value, customVariables, errors);
                }

                if (root[key].value is List<ParserItem>) // List type means that original JSON had an array at this position ...
                {
                    List<ParserItem> list = (List<ParserItem>)root[key].value;
                    for (int i = 0; i < list.Count; i++) // ... in that case we will do the same thing as above - but for every item of that array
                    {
                        if (list[i].value is string)
                        {
                            // replace variables, generate sequences, random values etc...
                            list[i] = applyFunctions(list[i], customVariables, errors);
                        }
                        if (list[i].value is Dictionary<string, ParserItem>) // Dictionary type means that original JSON had an array at this position
                        {
                            // call the same function recursively for the entire sub-node
                            parseInnerSyntaxOfRequestTemplate((Dictionary<string, ParserItem>)list[i].value, customVariables, errors);
                        }
                    }
                }
            }
        }

        private void parseInnerSyntaxOfResponseTemplate(Dictionary<string, ParserItem> root, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            List<string> keys = new List<string>(root.Keys);
            ParserCompareDefinition pcd;
            foreach (string key in keys)
            {

                if (root[key].value is string)
                {
                    // find out the exact comparing definiton from the value and save it along so we will be able to compare with actual received JSON response
                    root[key] = parseCompareRules(root[key], customVariables, errors);
                    continue;
                }
                
                if (root[key].value is Dictionary<string, ParserItem>) // Dictionary type means that original JSON had an object at this position
                {
                    // call the same function recursively for the entire sub-node
                    parseInnerSyntaxOfResponseTemplate((Dictionary<string, ParserItem>)root[key].value, customVariables, errors);
                    continue;
                }

                if (root[key].value is List<ParserItem>) // List type means that original JSON had an array at this position ...
                {
                    List<ParserItem> list = (List<ParserItem>)root[key].value;
                    if (list.Count > 0)
                    {
                        if (list[0].value is string) // if at least one item is present and it is string ...
                            list[0] = parseCompareRules(list[0], customVariables, errors); // ... parse compare rules for entire array from this field
                        // simple type
                        // make parser compare definition for strict comparison and assingn it to this item
                        pcd = new ParserCompareDefinition();
                        pcd.type = list[0].value.GetType();
                        pcd.value = list[0].value;
                        list[0].comp_def = pcd;
                    }
                    continue;
                }

                // simple type
                // make parser compare definition for strict comparison and assingn it to this item
                pcd = new ParserCompareDefinition();
                pcd.type = root[key].value.GetType();
                pcd.value = root[key].value;
                root[key].comp_def = pcd;
            }
        }

        private void compareResponseWithTemplate(Dictionary<string, ParserItem> template_root, Dictionary<string, ParserItem> response_root, Dictionary<string, string> customVariables, List<ParserError> errors, bool ignoreUndefinedKeys)
        {
            List<string> template_keys = new List<string>(template_root.Keys);
            List<string> response_keys = new List<string>(response_root.Keys);
            List<string> remaining_response_keys = new List<string>(response_root.Keys); // checklist for the response keys; every found match with template keys get one item deleted

            foreach (string template_key in template_keys)
            {
                if (response_root.ContainsKey(template_key))
                {
                    // we got a match between template key and response key; are their values of the same type?
                    if (response_root[template_key].value.GetType() == template_root[template_key].comp_def.value.GetType())
                    {
                        // values are of the same type
                        if (response_root[template_key].value is Dictionary<string, ParserItem>) // object
                        {
                            // call this method recursively in order to compare both sub-objects
                            compareResponseWithTemplate((Dictionary<string, ParserItem>)template_root[template_key].value, (Dictionary<string, ParserItem>)response_root[template_key].value, customVariables, errors, ignoreUndefinedKeys);
                        }
                        else if (response_root[template_key].value is List<ParserItem>) // array
                        {
                            if (template_root[template_key].comp_def.type == typeof(Array))
                            {
                                // we are going to compare number of items in the array with definition in template
                                int array_length = ((List<ParserItem>)response_root[template_key].value).Count;
                                compareParserItems(template_root[template_key], new ParserItem(response_root[template_key].line, response_root[template_key].position, array_length), customVariables, errors);
                            }
                            else
                            {
                                // we are going to compare each item in the array with definition in template
                                foreach (ParserItem item in (List<ParserItem>)response_root[template_key].value)
                                {
                                    compareParserItems(template_root[template_key], item, customVariables, errors);
                                }
                            }
                        }
                        else // simple type
                        {
                            compareParserItems(template_root[template_key], response_root[template_key], customVariables, errors);
                        }
                    }
                    else
                    {
                        // values are not of the same type => create error
                        errors.Add(new ParserError(template_root[template_key].line, template_root[template_key].position, string.Format(ERR_MSG_VALUES_DIFFER_IN_TYPE, template_key, response_root[template_key].value.GetType().ToString(),template_root[template_key].comp_def.value.GetType().ToString()), SOURCE_TEMPLATE));
                    }
                    remaining_response_keys.Remove(template_key); // remove matched key from remaining response keys
                }
                else
                {
                    // response does not contain this key - is this key defined as mandatory?
                    if (!template_root[template_key].comp_def.if_present)
                    {
                        // key is defined as mandatory and it is missing in the response => create error
                        errors.Add(new ParserError(template_root[template_key].line, template_root[template_key].position, string.Format(ERR_MSG_MISSING_KEY_IN_RESPONSE, template_key), SOURCE_TEMPLATE));
                    }
                }
            }

            // if corresponding flag is set to false every remaining unpaired keys in response are a problem => create errors
            if (!ignoreUndefinedKeys)
            {
                foreach (string response_key in remaining_response_keys)
                {
                    errors.Add(new ParserError(response_root[response_key].line, response_root[response_key].position, string.Format(ERR_MSG_ILLEGAL_KEY_IN_RESPONSE, response_key), SOURCE_RESPONSE));
                }
            }
        }

        private bool compareParserItems(ParserItem template_item, ParserItem response_item, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            // strict comparison
            if (template_item.comp_def.type == null && template_item.comp_def.operation == null && template_item.comp_def.value != null)
            {
                if (template_item.value.Equals(response_item.value))
                    return true;
                else
                {
                    errors.Add(new ParserError(template_item.line, template_item.position, string.Format(ERR_MSG_COMPARE_VALUES, template_item.value, response_item.value), SOURCE_TEMPLATE));
                    return false;
                }
            }

            // type comparison
            if (template_item.comp_def.type != null && template_item.comp_def.operation == null && template_item.comp_def.value == null)
            {
                if (template_item.comp_def.type == response_item.value.GetType())
                    return true;
                else
                {
                    errors.Add(new ParserError(template_item.line, template_item.position, string.Format(ERR_MSG_COMPARE_TYPES, template_item.comp_def.type, response_item.value.GetType()), SOURCE_TEMPLATE));
                    return false;
                }
            }

            // combined comparison (strict + type)
            if (template_item.comp_def.type != null && template_item.comp_def.operation == null && template_item.comp_def.value != null)
            {
                if ((template_item.value.Equals(response_item.value)) && (template_item.comp_def.type == response_item.value.GetType()))
                    return true;
                else
                {
                    if (template_item.value.Equals(response_item.value))
                        errors.Add(new ParserError(template_item.line, template_item.position, string.Format(ERR_MSG_COMPARE_VALUES, template_item.value, response_item.value), SOURCE_TEMPLATE));
                    else
                        errors.Add(new ParserError(template_item.line, template_item.position, string.Format(ERR_MSG_COMPARE_TYPES, template_item.comp_def.type, response_item.value.GetType()), SOURCE_TEMPLATE));
                    return false;
                }
            }

            // specific comparison (operation)
            if (template_item.comp_def.type != null && template_item.comp_def.operation != null && template_item.comp_def.value != null)
            {

            }

            return false;
        }

        private ParserItem parseCompareRules(ParserItem item, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            string value = (string)item.value;
            int original_error_count = errors.Count;

            // find out all positions of MAIN_SYNTAX_CHARACTERs in the string value
            List<int> occurrences = new List<int>();
            int index = 0;
            do
            {
                index = value.IndexOf(MAIN_SYNTAX_CHARACTER, index); // ... so, find all occurrences of function characters in given string value
                if (index != -1)
                {
                    if (index == 0 || value[index - 1] != ESCAPE_CHARACTER) // either first character or unescaped one
                        occurrences.Add(index);
                    index++;
                }
            } while (index != -1);
            List<int> original_occurrences = new List<int>(occurrences); // when we notify the user about parser error we want to refer to the positions from original template

            // take care of possibility that we got a plain string without any MAIN_SYNTAX_CHARACTERs
            if (occurrences.Count == 0)
            {
                item.comp_def = new ParserCompareDefinition();
                item.comp_def.type = typeof(string);
                item.comp_def.value = value;
                return item;
            }

            // take care of possible errors
            if (occurrences.Count < 5) // not enough MAIN_SYNTAX_CHARACTERs
                errors.Add(new ParserError(item.line, item.position + original_occurrences.Last(), string.Format(ERR_MSG_EXPECTING_CHARACTER, MAIN_SYNTAX_CHARACTER), SOURCE_TEMPLATE));
            if (occurrences.Count > 5) // too many MAIN_SYNTAX_CHARACTERs
                errors.Add(new ParserError(item.line, item.position + original_occurrences[5], string.Format(ERR_MSG_INVALID_CHARACTER, MAIN_SYNTAX_CHARACTER), SOURCE_TEMPLATE));
            if (occurrences.First() != 0) // illegal characters before first MAIN_SYNTAX_CHARACTER
                errors.Add(new ParserError(item.line, item.position, string.Format(ERR_MSG_INVALID_CHARACTER, value[0]), SOURCE_TEMPLATE));
            if (occurrences.Last() != value.Length - 1) // illegal characters after last MAIN_SYNTAX_CHARACTER
                errors.Add(new ParserError(item.line, item.position + original_occurrences.Last(), string.Format(ERR_MSG_INVALID_CHARACTER, value[original_occurrences.Last() + 1]), SOURCE_TEMPLATE));

            // if there are any new syntax errors we can't parse this item - return it without any change
            if (errors.Count > original_error_count)
                return item;

            // now  we can replace all escaped MAIN_SYNTAX_CHARACTER for the real ones
            // note: all concerned indices in occurrences list must be modified accordingly, since ESCAPE_CHARACTERs are being taken
            // out from the original string
            index = 0;
            do
            {
                index = value.IndexOf("" + ESCAPE_CHARACTER + MAIN_SYNTAX_CHARACTER, index); // find escaped MAIN_SYNTAX_CHARACTER
                if (index != -1)
                {
                    value = value.Remove(index, 1); // remove ESCAPE_CHARACTER from the string
                    // all occurrences with further placement than index of escaped character must be corrected by one
                    shift_occurrences(occurrences, index, -1);
                    index++;
                }
            } while (index != -1);

            //
            // retrieve individual values between MAIN_SYNTAX_CHARACTERs and save them to the item structure
            //
            string type = value.Substring(occurrences[0] + 1, occurrences[1] - occurrences[0] - 1); // type
            string[] ops = value.Substring(occurrences[1] + 1, occurrences[2] - occurrences[1] - 1).Split(RESPONSE_OPERATION_SEPARATOR); // operation with possible modifiers
            string val = value.Substring(occurrences[2] + 1, occurrences[3] - occurrences[2] - 1); // value
            string var = value.Substring(occurrences[3] + 1, occurrences[4] - occurrences[3] - 1); // variable
            ParserCompareDefinition pcd = new ParserCompareDefinition();

            // type
            if (type.Length > 0)
            {
                switch (type)
                {
                    case RESPONSE_TYPE_STRING:
                        pcd.type = typeof(string);
                        break;
                    case RESPONSE_TYPE_INTEGER:
                        pcd.type = typeof(int);
                        break;
                    case RESPONSE_TYPE_FLOAT:
                        pcd.type = typeof(float);
                        break;
                    case RESPONSE_TYPE_BOOLEAN:
                        pcd.type = typeof(bool);
                        break;
                    case RESPONSE_TYPE_OBJECT:
                        pcd.type = typeof(object);
                        break;
                    case RESPONSE_TYPE_ARRAY:
                        pcd.type = typeof(Array);
                        break;
                    default: // urecognized type
                        errors.Add(new ParserError(item.line, item.position + 1, string.Format(ERR_MSG_UNKNOWN_TYPE, type), SOURCE_TEMPLATE));
                        break;
                }
            }

            // operation + modifiers
            bool use_var = false; // use variable modifier
            bool if_present = false; // if present modifier
            string op = null; // operation
            if (ops.Length > 0)
            {
                foreach (string ops_item in ops)
                {
                    switch (ops_item)
                    {
                        case RESPONSE_OPERATION_MODIFIER_USE_VARIABLE:
                            use_var = true;
                            break;
                        case RESPONSE_OPERATION_MODIFIER_IF_PRESENT:
                            if_present = true;
                            break;
                        case RESPONSE_OPERATION_EQUAL:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_NOT_EQUAL:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_LESS_THAN:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_LESS_THAN_OR_EQUAL:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_GREATER_THAN:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_GREATER_THAN_OR_EQUAL:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        case RESPONSE_OPERATION_MATCHING_REGEXP_PATTERN:
                            if (RESPONSE_OPERATION_EQUAL_ALLOWED_TYPES.Contains(type))
                                op = ops_item;
                            else
                                errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_OPERATION_DOESNT_SUPPORT_TYPE, ops_item, type), SOURCE_TEMPLATE));
                            break;
                        default: // urecognized operation type
                            errors.Add(new ParserError(item.line, item.position + original_occurrences[1] + 1, string.Format(ERR_MSG_UNKNOWN_OPERATION, ops_item), SOURCE_TEMPLATE));
                            break;
                    }
                }

                // if operation was specified, type must be also specified
                if (pcd.type == null && pcd.operation != null)
                    errors.Add(new ParserError(item.line, item.position + 1, ERR_MSG_MISING_TYPE_FOR_OPERATION, SOURCE_TEMPLATE));

                // assign operation and modifiers into parser compare definition object
                pcd.use_variable = use_var;
                pcd.if_present = if_present;
                pcd.operation = op;
            }

            // value
            if (use_var) // do we want to replace variable name in value field by it's actual value?
            {
                if (customVariables.ContainsKey(val)) // search for desired custom variable ...
                {
                    // try to convert value of parsed variable name to desired type; if that is not possible add an error
                    pcd.value = convertToDesiredType(pcd.type, customVariables[val], item, errors, original_occurrences);
                }
                else // ... and add error if it doesn't exist
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_UNKNOWN_CUSTOM_VARIABLE, val), SOURCE_TEMPLATE));
            }
            else
            {
                // try to convert parsed value to desired type; if that is not possible add an error
                pcd.value = convertToDesiredType(pcd.type, val, item, errors, original_occurrences);
            }

            // variable
            pcd.var_name = var;

            // if there are any new syntax errors we can't parse this item - return it without any change
            if (errors.Count > original_error_count)
                return item;

            // assign newly constructed parser compare definition object into item and return the item
            item.comp_def = pcd;
            return item;
        }

        private object convertToDesiredType(System.Type desired_type, string value, ParserItem item, List<ParserError> errors, List<int> original_occurrences)
        {
            object result = null;

            if (desired_type == null)
            {
                // type was not provided => we will try to assume the closest type possible
                try
                {
                    result = Convert.ToInt32(value); // is it integer?
                }
                catch (Exception)
                {
                    try
                    {
                        result = Convert.ToSingle(value); // is it float?
                    }
                    catch (Exception)
                    {
                        if (value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_TRUE || value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_FALSE) // is it boolean?
                        {
                            if (value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_TRUE)
                                result = true;
                            else
                                result = false;
                        }
                        else
                        {
                            result = value; // let's assume that it's string
                        }
                    }
                }

            }
            else if (desired_type == typeof(string))
            {
                // we don't need to convert anything here, just assign the value
                result = value;
            }
            else if (desired_type == typeof(int) || desired_type == typeof(Array))
            {
                // try to convert value to integer
                try
                {
                    result = Convert.ToInt32(value);
                }
                catch (FormatException)
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_ARGUMENT_NOT_INTEGER, value), SOURCE_TEMPLATE));
                }
                catch (OverflowException)
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, value), SOURCE_TEMPLATE));
                }
            }
            else if (desired_type == typeof(float))
            {
                // try to convert value to float
                try
                {
                    result = Convert.ToSingle(value);
                }
                catch (FormatException)
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_ARGUMENT_NOT_FLOAT, value), SOURCE_TEMPLATE));
                }
                catch (OverflowException)
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, value), SOURCE_TEMPLATE));
                }
            }
            else if (desired_type == typeof(bool))
            {
                if (value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_TRUE || value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_FALSE)
                {
                    if (value == RESPONSE_BOOLEAN_CONVERSION_TYPE_DEFINITION_TRUE)
                        result = true;
                    else
                        result = false;
                }
                else
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_VALUE_UNCONVERTABLE_TO_TYPE, value, desired_type), SOURCE_TEMPLATE));
                }
            }
            else if (desired_type == typeof(object))
            {
                try
                {
                    result = deserialize(value);
                }
                catch (JsonException)
                {
                    errors.Add(new ParserError(item.line, item.position + original_occurrences[2] + 1, string.Format(ERR_MSG_VALUE_UNCONVERTABLE_TO_TYPE, value, desired_type), SOURCE_TEMPLATE));
                }
            }
            else
            {
                errors.Add(new ParserError(item.line, item.position + 1, string.Format(ERR_MSG_UNKNOWN_TYPE, desired_type), SOURCE_TEMPLATE));
            }

            return result;
        }

        private ParserItem applyFunctions(ParserItem item, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            string value = (string)item.value;

            // we need to replace variable names surrounded by VARIABLE_CHARACTERs with the actual variables ...
            List<int> var_occurrences = new List<int>();
            int index = 0;
            do
            {
                index = value.IndexOf(VARIABLE_CHARACTER, index); // ... so, find all occurrences of variable characters in given string value
                if (index != -1)
                {
                    if (index == 0 || value[index - 1] != ESCAPE_CHARACTER) // either first character or unescaped one
                        var_occurrences.Add(index);
                    index++;
                }
            } while (index != -1);
            List<int> original_var_occurrences = new List<int>(var_occurrences); // when we notify the user about parser error we want to refer to the positions from original template
            
            // we need to replace functions surrounded by MAIN_SYNTAX_CHARACTERs with their evaluated result ...
            List<int> fn_occurrences = new List<int>();
            index = 0;
            do
            {
                index = value.IndexOf(MAIN_SYNTAX_CHARACTER, index); // ... so, find all occurrences of function characters in given string value
                if (index != -1)
                {
                    if (index == 0 || value[index - 1] != ESCAPE_CHARACTER) // either first character or unescaped one
                        fn_occurrences.Add(index);
                    index++;
                }
            } while (index != -1);
            List<int> original_fn_occurrences = new List<int>(fn_occurrences); // when we notify the user about parser error we want to refer to the positions from original template

            if (var_occurrences.Count % 2 != 0)
            {
                // last VARIABLE_CHARACTER is missing it's pair ...
                errors.Add(new ParserError(item.line, item.position + original_var_occurrences.Last(), ERR_MSG_MISSING_VARIABLE_CHARACTER, SOURCE_TEMPLATE)); // ... notify the user ...
                var_occurrences.RemoveAt(var_occurrences.Count - 1); // ... and do not try to replace this one in future
            }

            if (fn_occurrences.Count % 2 != 0)
            {
                // last MAIN_SYNTAX_CHARACTER is missing it's pair ...
                errors.Add(new ParserError(item.line, item.position + original_fn_occurrences.Last(), ERR_MSG_MISSING_MAIN_SYNTAX_CHARACTER, SOURCE_TEMPLATE)); // ... notify the user ...
                fn_occurrences.RemoveAt(fn_occurrences.Count - 1); // ... and do not try to replace this one in future
            }

            // now when we have identified all variable occurences, we can replace all escaped VARIABLE_CHARACTER for the real ones
            // note: all concerned indices in occurrences list must be modified accordingly, since ESCAPE_CHARACTERs are being taken
            // out from the original string
            index = 0;
            do
            {
                index = value.IndexOf("" + ESCAPE_CHARACTER + VARIABLE_CHARACTER, index); // find escaped VARIABLE_CHARACTER
                if (index != -1)
                {
                    value = value.Remove(index, 1); // remove ESCAPE_CHARACTER from the string
                    // all occurrences with further placement than index of escaped character must be corrected by one
                    shift_occurrences(var_occurrences, index, -1);
                    shift_occurrences(fn_occurrences, index, -1);
                    index++;
                }
            } while (index != -1);

            // now when we have identified all function occurences, we can replace all escaped MAIN_SYNTAX_CHARACTER for the real ones
            // note: all concerned indices in occurrences list must be modified accordingly, since ESCAPE_CHARACTERs are being taken
            // out from the original string
            index = 0;
            do
            {
                index = value.IndexOf("" + ESCAPE_CHARACTER + MAIN_SYNTAX_CHARACTER, index); // find escaped MAIN_SYNTAX_CHARACTER
                if (index != -1)
                {
                    value = value.Remove(index, 1); // remove ESCAPE_CHARACTER from the string
                    // all occurrences with further placement than index of escaped character must be corrected by one
                    shift_occurrences(var_occurrences, index, -1);
                    shift_occurrences(fn_occurrences, index, -1);
                    index++;
                }
            } while (index != -1);

            // here we need to find out whether VARIABLE_CHARACTER and MAIN_SYNTAX_CHARACTER pairs are not overlaping each other in forbidden way
            // RULES:
            //  - MAIN_SYNTAX_CHARACTER pair can have multiple VARIABLE_CHARACTER pairs inside it
            //  - VARIABLE_CHARACTER pair can not any MAIN_SYNTAX_CHARACTER pairs inside it
            //  - MAIN_SYNTAX_CHARACTER pair can not overlap with VARIABLE_CHARACTER pair and vice versa
            if (var_occurrences.Count > 0 && fn_occurrences.Count > 0) // both pair lists must be non empty for this check to make any sense
            {
                int var_list_position = 0;
                int fn_list_position = 0;
                bool inside_var = false; // whether current position is inside MAIN_SYNTAX_CHARACTER pair or not
                bool inside_fn = false; // whether current position is inside VARIABLE_CHARACTER pair or not

                while (var_list_position < var_occurrences.Count && fn_list_position < fn_occurrences.Count)
                {
                    // current position in both pair lists is not at the end
                    if (var_occurrences[var_list_position] < fn_occurrences[fn_list_position])
                    {
                        inside_var = !inside_var;
                        var_list_position++;
                    }
                    else
                    {
                        if (inside_var && !inside_fn)
                        {
                            // there is MAIN_SYNTAX_CHARACTER inside VARIABLE_CHARACTERs pair - raise an error
                            errors.Add(new ParserError(item.line, item.position + fn_occurrences[fn_list_position], ERR_MSG_FN_INSIDE_VAR, SOURCE_TEMPLATE));
                            // this syntax error is too serious for evaluation to continue - return original item without any change
                            return item;
                        }

                        inside_fn = !inside_fn;
                        fn_list_position++;
                    }
                }
                // upon leaving this while statement, we can be sure that we made it through at least one of the lists
                // the rest does not matter now, since there can't be any more overlaps or bad placements
            }

            // try to match variable names with dictionary of custom variables
            // notify the user if match is not found
            for (int i = 0; i < var_occurrences.Count; i += 2)
            {
                string variable_name = value.Substring(var_occurrences[i] + 1, var_occurrences[i + 1] - var_occurrences[i] - 1); // grab variable name without VARIABLE_CHARACTERs on edges
                if (customVariables.ContainsKey(variable_name)) // is this variable defined in passed custom variables?
                {
                    // replace variable name by it's value
                    value = value.Remove(var_occurrences[i], var_occurrences[i + 1] - var_occurrences[i] + 1); // remove variable name from original string inc. VARIABLE_CHARACTERs on edges
                    value = value.Insert(var_occurrences[i], customVariables[variable_name]); // insert actual value of that variable on the original position
                    // after we changed length of original string by replacing variable name by it's value, we need to change all occurrence indices accordingly
                    shift_occurrences(var_occurrences, var_occurrences[i], customVariables[variable_name].Length - variable_name.Length - 2);
                    shift_occurrences(fn_occurrences, var_occurrences[i], customVariables[variable_name].Length - variable_name.Length - 2);
                }
                else
                {
                    // create new error
                    errors.Add(new ParserError(item.line, item.position + original_var_occurrences[i], string.Format(ERR_MSG_UNKNOWN_CUSTOM_VARIABLE, variable_name), SOURCE_TEMPLATE));
                }
            }

            // now, we are ready to find and evaluate functions defined in this string
            for (int i = 0; i < fn_occurrences.Count; i += 2)
            {
                string function_string = value.Substring(fn_occurrences[i] + 1, fn_occurrences[i + 1] - fn_occurrences[i] - 1); // grab the function name and it's parameters without MAIN_SYNTAX_CHARACTERs on edges

                int left_parenthesis_position = function_string.IndexOf(REQUEST_FN_LEFT_PARENTHESIS);
                int right_parenthesis_position = function_string.IndexOf(REQUEST_FN_RIGHT_PARENTHESIS);

                // handle all possible syntactical errors
                if (left_parenthesis_position == -1) // no left parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], string.Format(ERR_MSG_EXPECTING_CHARACTER, REQUEST_FN_LEFT_PARENTHESIS), SOURCE_TEMPLATE));
                if (right_parenthesis_position == -1) // no right parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], string.Format(ERR_MSG_EXPECTING_CHARACTER, REQUEST_FN_RIGHT_PARENTHESIS), SOURCE_TEMPLATE));
                if (left_parenthesis_position != -1 && function_string.IndexOf(REQUEST_FN_LEFT_PARENTHESIS, left_parenthesis_position) == -1) // too many left parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + left_parenthesis_position, string.Format(ERR_MSG_INVALID_CHARACTER, REQUEST_FN_LEFT_PARENTHESIS), SOURCE_TEMPLATE));
                if (right_parenthesis_position != -1 && function_string.IndexOf(REQUEST_FN_RIGHT_PARENTHESIS, right_parenthesis_position) == -1) // too many right parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, string.Format(ERR_MSG_INVALID_CHARACTER, REQUEST_FN_RIGHT_PARENTHESIS), SOURCE_TEMPLATE));
                if (left_parenthesis_position == 0) // no function name before left parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], ERR_MSG_MISSING_FN_NAME, SOURCE_TEMPLATE));
                if (right_parenthesis_position != function_string.Length - 1) // aditional characters after right parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, ERR_MSG_UNRECOGNIZED_CHARS_AFTER_FN, SOURCE_TEMPLATE));
                if (left_parenthesis_position > right_parenthesis_position) // swapped parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, ERR_MSG_SWAPPED_PARENTHESES, SOURCE_TEMPLATE));

                // if all basic syntactical conditions are met, we can continue
                if (left_parenthesis_position > 0 && left_parenthesis_position < right_parenthesis_position && right_parenthesis_position == function_string.Length - 1)
                {
                    string function_name = function_string.Substring(0, left_parenthesis_position);
                    string[] function_arguments = function_string.Substring(left_parenthesis_position + 1, right_parenthesis_position - left_parenthesis_position - 1).Split(REQUEST_FN_ARGUMENT_SEPARATOR);
                    for (int j = 0; j < function_arguments.Length; j++)
                        function_arguments[j] = function_arguments[j].Trim(); // trim any white spaces from all arguments
                    string function_result = null;
                    bool type_result = false; // if the entire "value" string is occupied only by one function, we will type the result from string to desired variable type (integer of float)

                    // try to recognize the function name
                    switch (function_name)
                    {
                        case REQUEST_FN_RAND_INT:
                            function_result = applyRequestFunctionRandInt(function_arguments, errors, item, fn_occurrences[i]);
                            if (fn_occurrences.Count == 2 && fn_occurrences[0] == 0 && fn_occurrences[1] == value.Length - 1) // the entire "value" string is occupied only by one function
                                type_result = true; // we will type the result to integer
                            break;
                        case REQUEST_FN_RAND_FLOAT:
                            function_result = applyRequestFunctionRandFloat(function_arguments, errors, item, fn_occurrences[i]);
                            if (fn_occurrences.Count == 2 && fn_occurrences[0] == 0 && fn_occurrences[1] == value.Length - 1) // the entire "value" string is occupied only by one function
                                type_result = true; // we will type the result to float
                            break;
                        case REQUEST_FN_RAND_STRING:
                            function_result = applyRequestFunctionRandString(function_arguments, errors, item, fn_occurrences[i]);
                            break;
                        case REQUEST_FN_SEQUENCE:
                            function_result = applyRequestFunctionSequence(function_arguments, errors, item, fn_occurrences[i], customVariables);
                            if (fn_occurrences.Count == 2 && fn_occurrences[0] == 0 && fn_occurrences[1] == value.Length - 1) // the entire "value" string is occupied only by one function
                                type_result = true; // we will type the result to integer
                            break;
                        default:
                            errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], ERR_MSG_UNRECOGNIZED_FN, SOURCE_TEMPLATE));
                            break;
                    }

                    // if there were no difficulties during function evaluation, we can continue
                    if (function_result != null)
                    {
                        // replace entire function string by the function result
                        value = value.Remove(fn_occurrences[i], fn_occurrences[i + 1] - fn_occurrences[i] + 1); // remove entire function string from original string inc. MAIN_SYNTAX_CHARACTERs on edges
                        value = value.Insert(fn_occurrences[i], function_result); // insert function result on the original position
                        // after we changed length of original string by replacing entire function string by function result, we need to change all occurrence indices accordingly
                        shift_occurrences(fn_occurrences, fn_occurrences[i], function_result.Length - function_string.Length - 2);
                    }

                    if (type_result) // the entire "value" string is occupied only by one function, we must type the result to desired one
                    {
                        switch (function_name)
                        {
                            case REQUEST_FN_RAND_INT:
                                item.value = Convert.ToInt32(value); // type to integer
                                break;
                            case REQUEST_FN_RAND_FLOAT:
                                item.value = Convert.ToSingle(value); // type to float
                                break;
                            case REQUEST_FN_SEQUENCE:
                                item.value = Convert.ToInt32(value); // type to integer
                                break;
                            default:
                                item.value = value;
                                break;
                        }
                    }
                    else // don't type result to differen type and assign regular string
                    {
                        item.value = value;
                    }
                }
            }

            // return modified item
            return item;
        }

        /// <summary>
        /// This auxiliary method change all list items, which are above threshold value, by shift value.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="threshold"></param>
        /// <param name="shift"></param>
        private void shift_occurrences(List<int> list, int threshold, int shift)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > threshold)
                    list[i] += shift;
            }
        }

        /// <summary>
        /// This auxiliary method tries to convert string argument to integer and if it fails to do so error is added to the errors list.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <returns></returns>
        private int parse_integer_function_argument(string argument, List<ParserError> errors, ParserItem item, int parsing_offset)
        {
            int result = Int32.MinValue;
            try
            {
                result = Convert.ToInt32(argument);
            }
            catch (FormatException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_NOT_INTEGER, argument), SOURCE_TEMPLATE));
            }
            catch (OverflowException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, argument), SOURCE_TEMPLATE));
            }
            return result;
        }

        /// <summary>
        /// This auxiliary method tries to convert string argument to float and if it fails to do so error is added to the errors list.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <returns></returns>
        private float parse_float_function_argument(string argument, List<ParserError> errors, ParserItem item, int parsing_offset)
        {
            float result = Single.MinValue;
            try
            {
                result = Convert.ToSingle(argument);
            }
            catch (FormatException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_NOT_FLOAT, argument), SOURCE_TEMPLATE));
            }
            catch (OverflowException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, argument), SOURCE_TEMPLATE));
            }
            return result;
        }

        /// <summary>
        /// This auxiliary method tries to parse passed string arguments and generate random integer according to specification.
        /// </summary>
        /// <param name="function_arguments"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <returns></returns>
        private string applyRequestFunctionRandInt(string[] function_arguments, List<ParserError> errors, ParserItem item, int parsing_offset)
        {
            // check number of arguments
            if (function_arguments.Length != 2)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS, SOURCE_TEMPLATE));
                return null;
            }

            // parse arguments
            int original_errors_count = errors.Count;
            int min_range = parse_integer_function_argument(function_arguments[0], errors, item, parsing_offset); // parse first argument
            int max_range = parse_integer_function_argument(function_arguments[1], errors, item, parsing_offset); // parse second argument
            if (errors.Count > original_errors_count) // if some new errors occured during parsing of the arguments ...
                return null; // ... stop evaluating and leave this code block

            // check whether first arg. is smaller or equal that the second one
            if (min_range > max_range)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2]), SOURCE_TEMPLATE));
                return null;
            }

            // generate random integer between the value of first and second argument and return it as a string
            return "" + this.random.Next(min_range, max_range + 1);
        }

        /// <summary>
        /// This auxiliary method tries to parse passed string arguments and generate random float according to specification.
        /// </summary>
        /// <param name="function_arguments"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <returns></returns>
        private string applyRequestFunctionRandFloat(string[] function_arguments, List<ParserError> errors, ParserItem item, int parsing_offset)
        {
            // check number of arguments
            if (function_arguments.Length != 3)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS, SOURCE_TEMPLATE));
                return null;
            }

            // parse arguments
            int original_errors_count = errors.Count;
            float min_range = parse_float_function_argument(function_arguments[0], errors, item, parsing_offset); // parse first argument
            float max_range = parse_float_function_argument(function_arguments[1], errors, item, parsing_offset); // parse second argument
            int precision = parse_integer_function_argument(function_arguments[2], errors, item, parsing_offset); // parse third argument
            if (errors.Count > original_errors_count) // if some new errors occured during parsing of the arguments ...
                return null; // ... stop evaluating and leave this code block

            // check whether first arg. is smaller or equal that the second one
            if (min_range > max_range)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2]), SOURCE_TEMPLATE));
                return null;
            }

            // generate random float between the value of first and second argument with desired number of fractional digits and return it as a string
            return "" + Math.Round((this.random.NextDouble() * (max_range - min_range) + min_range), precision);
        }

        /// <summary>
        /// This auxiliary method tries to parse passed string arguments and generate random string according to specification.
        /// </summary>
        /// <param name="function_arguments"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <returns></returns>
        private string applyRequestFunctionRandString(string[] function_arguments, List<ParserError> errors, ParserItem item, int parsing_offset)
        {
            string character_pool = ""; // this string will eventually contain all desired characters from which we will generate resulting random string

            // check number of arguments
            if (function_arguments.Length != 3)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS, SOURCE_TEMPLATE));
                return null;
            }

            // parse arguments
            int original_errors_count = errors.Count;
            int min_length = parse_integer_function_argument(function_arguments[0], errors, item, parsing_offset); // parse first argument
            int max_length = parse_integer_function_argument(function_arguments[1], errors, item, parsing_offset); // parse second argument
            foreach (char ch in function_arguments[2]) // parse third argument
            {
                if (!REQUEST_FN_RAND_STRING_GROUP_SYMBOLS.Contains(ch)) // each character of third argument must be recognized as symbol for group of characters
                {
                    // symbol for group of characters was not recognized - add an error
                    errors.Add(new ParserError(item.line, item.position + parsing_offset, String.Format(ERR_MSG_ARGUMENT_INVALID, function_arguments[2]), SOURCE_TEMPLATE));
                    break;
                }
                else
                {
                    // symbol for group of characters was recognized - add set of characters represented by this symbol to the character pool
                    character_pool += REQUEST_FN_RAND_STRING_GROUPS[Array.FindIndex(REQUEST_FN_RAND_STRING_GROUP_SYMBOLS, x => x == ch)];
                }
            }
            if (errors.Count > original_errors_count) // if some new errors occured during parsing of the arguments ...
                return null; // ... stop evaluating and leave this code block

            // check whether first arg. is smaller or equal that the second one
            if (min_length > max_length)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2]), SOURCE_TEMPLATE));
                return null;
            }

            // generate random string between the lenght of first and second argument with desired type of characters and return it as a string
            int lenght = this.random.Next(min_length, max_length + 1);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lenght; i++)
               sb.Append(character_pool[this.random.Next(character_pool.Length)]);
            return sb.ToString();
        }

        /// <summary>
        /// This auxiliary method tries to parse passed string arguments and generate next integer value of particular sequence according to specification.
        /// </summary>
        /// <param name="function_arguments"></param>
        /// <param name="errors"></param>
        /// <param name="item"></param>
        /// <param name="parsing_offset"></param>
        /// <param name="customVariables"></param>
        /// <returns></returns>
        private string applyRequestFunctionSequence(string[] function_arguments, List<ParserError> errors, ParserItem item, int parsing_offset, Dictionary<string, string> customVariables)
        {
            bool new_sequence = false; // this variable determines whether we do need to create new custom variable to hold current value of the sequence
            int current_value = Int32.MinValue;

            // check number of arguments
            if (function_arguments.Length != 4)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS, SOURCE_TEMPLATE));
                return null;
            }

            // parse arguments
            int original_errors_count = errors.Count;
            int start = parse_integer_function_argument(function_arguments[0], errors, item, parsing_offset); // parse first argument
            int end = function_arguments[1].Length>0 ? parse_integer_function_argument(function_arguments[1], errors, item, parsing_offset) : Int32.MinValue; // parse second argument (might be empty)
            int increment = parse_integer_function_argument(function_arguments[2], errors, item, parsing_offset); // parse third argument
            // parse fourth argument
            if (!customVariables.ContainsKey(function_arguments[3]))
            {
                new_sequence = true; // unknown custom variable - set up a new custom variable containing start of the sequence below
            }
            else
            {
                // replace custom variable name by it's value
                function_arguments[3] = customVariables[function_arguments[3]];
                current_value = parse_integer_function_argument(function_arguments[3], errors, item, parsing_offset); // value of custom variable passed in 4th argument must contain integer value
            }
            if (errors.Count > original_errors_count) // if some new errors occured during parsing of the arguments ...
                return null; // ... stop evaluating and leave this code block

            // generate a new number of this sequence based on passed arguments and return it as a string
            int next_sequence_number;
            if (new_sequence)
            {
                customVariables.Add(function_arguments[3], Convert.ToString(start));
                next_sequence_number = start;
            }
            else
            {
                if (end == Int32.MinValue)
                {
                    // second argument was not provided - simply add increment to current value
                    next_sequence_number = current_value + increment;
                }
                else
                {
                    // second argument was provided - add increment to current value, but loop it from start if it exceeds end
                    next_sequence_number = current_value + increment;
                    if ((increment > 0 && next_sequence_number > end) || (increment < 0 && next_sequence_number < end))
                        next_sequence_number = start; // reset sequence
                }
                customVariables[function_arguments[3]] = Convert.ToString(next_sequence_number); // save new sequence value for next request
            }
            return "" + next_sequence_number;
        }

    }
}
