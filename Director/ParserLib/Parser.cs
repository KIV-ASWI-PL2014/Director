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

namespace Director.ParserLib
{
    public class Parser
    {
        // general syntactical characters
        private const char VARIABLE_CHARACTER = '$';
        private const char MAIN_SYNTAX_CHARACTER = '#';
        private const char ESCAPE_CHARACTER = '\\';

        // request specific constants
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

        // parser error messages
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

        // global variables
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

            // try to deserialize JSON template into internal parser structures
            try
            {
                root = deserialize(template);
            }
            catch (JsonException e)
            {
                // an error occured during deserializon => basic syntax of json template is wrong
                errors.Add(createError(e.Message));
                // add error to the error list and return empty result
                return new ParserResult(errors, null);
            }

            // parse inner syntax written in value of some tags (replace variables, evaluate functions etc...)
            parseInnerSyntaxOfRequest(root, customVariables, errors);

            // serialize JSON string back from modified internal parser structures
            string result = serialize(root);

            // return the result along with some potential non-critical errors
            return new ParserResult(errors, result);
        }

        public ParserResult validateResponse(string template, Dictionary<string, string> customVariables)
        {
            return null;
        }

        public ParserResult parseResponse(string template, string response, Dictionary<string, string> customVariables)
        {
            return null;
        }

        private ParserError createError(string errorMessage)
        {
            const string LINE_DEF = ", line ";
            const string POSITION_DEF = ", position ";

            int posLine = errorMessage.IndexOf(LINE_DEF);
            int posPosition = errorMessage.IndexOf(POSITION_DEF);
            if ( (posLine != -1) && (posPosition != -1) )
            {
                int line = Convert.ToInt32(Regex.Match(errorMessage.Substring(posLine, posPosition - posLine), @"\d+").Value);
                int position = Convert.ToInt32(Regex.Match(errorMessage.Substring(posPosition, errorMessage.Length - posPosition), @"\d+").Value);
                return new ParserError(line, position, errorMessage.Substring(0, posPosition - LINE_DEF.Length - 1));
            }
            return new ParserError(-1, -1, errorMessage);
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

        private void parseInnerSyntaxOfRequest(Dictionary<string, ParserItem> root, Dictionary<string, string> customVariables, List<ParserError> errors)
        {
            List<string> keys = new List<string>(root.Keys);
            foreach (string key in keys)
            {

                if (root[key].value is string)
                {
                    // replace variables, generate sequences, random values etc...
                    root[key] = applyFunctions(root[key], customVariables, errors);
                }
                if (root[key].value is Dictionary<string, ParserItem>) // Dictionary type means that original JSON had an array at this position
                {
                    // call the same function recursively for the entire sub-node
                    parseInnerSyntaxOfRequest((Dictionary<string, ParserItem>)root[key].value, customVariables, errors);
                }

                if (root[key].value is List<ParserItem>) // List type means that original JSON had an array at this position ...
                {
                    List<ParserItem> list = (List<ParserItem>)root[key].value;
                    foreach (ParserItem item in list) // ... in that case we will do the same thing as above - but for every item of that array
                    {
                        if (root[key].value is string)
                        {
                            // replace variables, generate sequences, random values etc...
                            root[key] = applyFunctions(root[key], customVariables, errors);
                        }
                        if (root[key].value is Dictionary<string, ParserItem>) // Dictionary type means that original JSON had an array at this position
                        {
                            // call the same function recursively for the entire sub-node
                            parseInnerSyntaxOfRequest((Dictionary<string, ParserItem>)root[key].value, customVariables, errors);
                        }
                    }
                }
            }
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
                    // Strnadj: Pokud je to 1 znak, tzn 0, nemùže být escapována..
                    if (index == 0 || value[index - 1] != ESCAPE_CHARACTER)
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
                    // Strnadj: Pokud je to 1 znak, tzn 0, nemùže být escapována..
                    if (index == 0 || value[index - 1] != ESCAPE_CHARACTER) 
                        fn_occurrences.Add(index);
                    index++;
                }
            } while (index != -1);
            List<int> original_fn_occurrences = new List<int>(fn_occurrences); // when we notify the user about parser error we want to refer to the positions from original template

            if (var_occurrences.Count % 2 != 0)
            {
                // last VARIABLE_CHARACTER is missing it's pair ...
                errors.Add(new ParserError(item.line, item.position + original_var_occurrences.Last(), ERR_MSG_MISSING_VARIABLE_CHARACTER)); // ... notify the user ...
                var_occurrences.RemoveAt(var_occurrences.Count - 1); // ... and do not try to replace this one in future
            }

            if (fn_occurrences.Count % 2 != 0)
            {
                // last MAIN_SYNTAX_CHARACTER is missing it's pair ...
                errors.Add(new ParserError(item.line, item.position + original_fn_occurrences.Last(), ERR_MSG_MISSING_MAIN_SYNTAX_CHARACTER)); // ... notify the user ...
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
                            errors.Add(new ParserError(item.line, item.position + fn_occurrences[fn_list_position], ERR_MSG_FN_INSIDE_VAR));
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
                    errors.Add(new ParserError(item.line, item.position + original_var_occurrences[i], string.Format(ERR_MSG_UNKNOWN_CUSTOM_VARIABLE, variable_name)));
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
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], string.Format(ERR_MSG_EXPECTING_CHARACTER, REQUEST_FN_LEFT_PARENTHESIS)));
                if (right_parenthesis_position == -1) // no right parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], string.Format(ERR_MSG_EXPECTING_CHARACTER, REQUEST_FN_RIGHT_PARENTHESIS)));
                if (left_parenthesis_position != -1 && function_string.IndexOf(REQUEST_FN_LEFT_PARENTHESIS, left_parenthesis_position) == -1) // too many left parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + left_parenthesis_position, string.Format(ERR_MSG_INVALID_CHARACTER, REQUEST_FN_LEFT_PARENTHESIS)));
                if (right_parenthesis_position != -1 && function_string.IndexOf(REQUEST_FN_RIGHT_PARENTHESIS, right_parenthesis_position) == -1) // too many right parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, string.Format(ERR_MSG_INVALID_CHARACTER, REQUEST_FN_RIGHT_PARENTHESIS)));
                if (left_parenthesis_position == 0) // no function name before left parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], ERR_MSG_MISSING_FN_NAME));
                if (right_parenthesis_position != function_string.Length - 1) // aditional characters after right parenthesis
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, ERR_MSG_UNRECOGNIZED_CHARS_AFTER_FN));
                if (left_parenthesis_position > right_parenthesis_position) // swapped parentheses
                    errors.Add(new ParserError(item.line, item.position + fn_occurrences[i] + right_parenthesis_position, ERR_MSG_SWAPPED_PARENTHESES));

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
                            errors.Add(new ParserError(item.line, item.position + fn_occurrences[i], ERR_MSG_UNRECOGNIZED_FN));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_NOT_INTEGER, argument)));
            }
            catch (OverflowException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, argument)));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_NOT_FLOAT, argument)));
            }
            catch (OverflowException)
            {
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENT_TOO_BIG, argument)));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2])));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2])));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS));
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
                    errors.Add(new ParserError(item.line, item.position + parsing_offset, String.Format(ERR_MSG_ARGUMENT_INVALID, function_arguments[2])));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, string.Format(ERR_MSG_ARGUMENTS_WRONG_SIZED, 1, function_arguments[1], 2, function_arguments[2])));
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
                errors.Add(new ParserError(item.line, item.position + parsing_offset, ERR_MSG_INVALID_NUMBER_OF_ARGUMENTS));
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
