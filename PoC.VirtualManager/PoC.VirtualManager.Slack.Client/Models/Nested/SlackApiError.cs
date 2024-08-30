using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models.Nested
{
    public enum SlackApiError
    {
        /// <summary>
        /// Value passed for channel was invalid.
        /// </summary>
        ChannelNotFound,

        /// <summary>
        /// Value passed for cursor was not valid or is no longer valid.
        /// </summary>
        InvalidCursor,

        /// <summary>
        /// Value passed for metadata_keys_to_include was invalid. Must be valid JSON array of strings.
        /// </summary>
        InvalidMetadataFilterKeys,

        /// <summary>
        /// Value passed for latest was invalid.
        /// </summary>
        InvalidTsLatest,

        /// <summary>
        /// Value passed for oldest was invalid.
        /// </summary>
        InvalidTsOldest,

        /// <summary>
        /// The token used does not have access to the proper channel. Only user tokens can access public channels they are not in.
        /// </summary>
        NotInChannel,

        /// <summary>
        /// Pagination does not currently function on Enterprise Grid workspaces.
        /// </summary>
        PaginationNotAvailable,

        /// <summary>
        /// Access to a resource specified in the request is denied.
        /// </summary>
        AccessDenied,

        /// <summary>
        /// Authentication token is for a deleted user or workspace when using a bot token.
        /// </summary>
        AccountInactive,

        /// <summary>
        /// The endpoint has been deprecated.
        /// </summary>
        DeprecatedEndpoint,

        /// <summary>
        /// Administrators have suspended the ability to post a message.
        /// </summary>
        EkmAccessDenied,

        /// <summary>
        /// The method cannot be called from an Enterprise.
        /// </summary>
        EnterpriseIsRestricted,

        /// <summary>
        /// Some aspect of authentication cannot be validated. Either the provided token is invalid or the request originates from an IP address disallowed from making the request.
        /// </summary>
        InvalidAuth,

        /// <summary>
        /// The method has been deprecated.
        /// </summary>
        MethodDeprecated,

        /// <summary>
        /// The token used is not granted the specific scope permissions required to complete this request.
        /// </summary>
        MissingScope,

        /// <summary>
        /// The token type used in this request is not allowed.
        /// </summary>
        NotAllowedTokenType,

        /// <summary>
        /// No authentication token provided.
        /// </summary>
        NotAuthed,

        /// <summary>
        /// The workspace token used in this request does not have the permissions necessary to complete the request. Make sure your app is a member of the conversation it's attempting to post a message to.
        /// </summary>
        NoPermission,

        /// <summary>
        /// The workspace is undergoing an enterprise migration and will not be available until migration is complete.
        /// </summary>
        OrgLoginRequired,

        /// <summary>
        /// Authentication token has expired.
        /// </summary>
        TokenExpired,

        /// <summary>
        /// Authentication token is for a deleted user or workspace or the app has been removed when using a user token.
        /// </summary>
        TokenRevoked,

        /// <summary>
        /// Two-factor setup is required.
        /// </summary>
        TwoFactorSetupRequired,

        /// <summary>
        /// The token used is not granted the specific workspace access required to complete this request.
        /// </summary>
        TeamAccessNotGranted,

        /// <summary>
        /// Access to this method is limited on the current network.
        /// </summary>
        AccessLimited,

        /// <summary>
        /// The server could not complete your operation(s) without encountering a catastrophic error. It's possible some aspect of the operation succeeded before the error was raised.
        /// </summary>
        FatalError,

        /// <summary>
        /// The server could not complete your operation(s) without encountering an error, likely due to a transient issue on our end. It's possible some aspect of the operation succeeded before the error was raised.
        /// </summary>
        InternalError,

        /// <summary>
        /// The method was passed an argument whose name falls outside the bounds of accepted or expected values. This includes very long names and names with non-alphanumeric characters other than _.
        /// </summary>
        InvalidArgName,

        /// <summary>
        /// The method was either called with invalid arguments or some detail about the arguments passed is invalid, which is more likely when using complex arguments like blocks or attachments.
        /// </summary>
        InvalidArguments,

        /// <summary>
        /// The method was passed an array as an argument. Please only input valid strings.
        /// </summary>
        InvalidArrayArg,

        /// <summary>
        /// The method was called via a POST request, but the charset specified in the Content-Type header was invalid. Valid charset names are: utf-8, iso-8859-1.
        /// </summary>
        InvalidCharset,

        /// <summary>
        /// The method was called via a POST request with Content-Type application/x-www-form-urlencoded or multipart/form-data, but the form data was either missing or syntactically invalid.
        /// </summary>
        InvalidFormData,

        /// <summary>
        /// The method was called via a POST request, but the specified Content-Type was invalid. Valid types are: application/json, application/x-www-form-urlencoded, multipart/form-data, text/plain.
        /// </summary>
        InvalidPostType,

        /// <summary>
        /// The method was called via a POST request and included a data payload, but the request did not include a Content-Type header.
        /// </summary>
        MissingPostType,

        /// <summary>
        /// The request has been rate-limited. Refer to the Retry-After header for when to retry the request.
        /// </summary>
        RateLimited,

        /// <summary>
        /// The method was called via a POST request, but the POST data was either missing or truncated.
        /// </summary>
        RequestTimeout,

        /// <summary>
        /// The service is temporarily unavailable.
        /// </summary>
        ServiceUnavailable,

        /// <summary>
        /// The workspace associated with your request is currently undergoing migration to an Enterprise Organization. Web API and other platform operations will be intermittently unavailable until the transition is complete.
        /// </summary>
        TeamAddedToOrg
    }

    public static class ErrorParser
    {
        public static SlackApiError ToEnum(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            // Convert the string to CamelCase (PascalCase)
            var camelCaseValue = string.Concat(value.Split('_')
                .Select(word => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower())));

            // Try to parse the CamelCase string to the enum value
            if (Enum.TryParse<SlackApiError>(camelCaseValue, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unable to convert '{value}' to enum '{typeof(SlackApiError).Name}'.");
        }
    }
}
