using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct ImageNames
    {
        internal const string PreImageName = "PreImage";
        internal const string PostImageName = "PostImage";
    }

    // TODO create tests
    public static class ImageExtensions
    {
        #region "Pre Image"
        /// <summary>
        /// WARNING: Prefer the parameterless overload unless a non-standard image name is required.
        /// Checks if there is a Pre Image Entity matching the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Pre Image Entity name</param>
        /// <returns>Boolean indicating if the Pre Image Entity exists</returns>
        public static bool HasPreImage(this IPluginExecutionContext context, string name)
        {
            return context.PreEntityImages.ContainsKey(name);
        }

        /// <summary>
        /// Checks if there is a Pre Image Entity matching the name "PreImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Boolean indicating if the Pre Image Entity exists</returns>
        public static bool HasPreImage(this IPluginExecutionContext context)
        {
            return context.PreEntityImages.ContainsKey(ImageNames.PreImageName);
        }

        /// <summary>
        /// WARNING: this method should be avoided and the GetPreImage() method used instead.
        /// Gets the Pre Image with the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Pre Image Entity name</param>
        /// <returns>The Pre Image Entity</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PreImage</exception>
        public static Entity GetPreImage(this IPluginExecutionContext context, string name, ITracingService tracer = null)
        {
            if (!context.HasPreImage(name))
            {
                throw new ArgumentException($"Pre Image with name \"{name}\" does not exist.");
            }

            var image = (Entity)context.PreEntityImages[name];

            tracer?.TraceEntity(image, name);

            return image;
        }

        /// <summary>
        /// Gets the Pre Image with the name "PreImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The Pre Image Entity</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PreImage</exception>
        public static Entity GetPreImage(this IPluginExecutionContext context, ITracingService tracer = null)
        {
            return context.GetPreImage(ImageNames.PreImageName, tracer);
        }

        #endregion

        #region "Post Image"
        /// <summary>
        /// WARNING: Prefer the parameterless overload unless a non-standard image name is required.
        /// Checks if there is a Post Image Entity matching the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Post Image Entity name</param>
        /// <returns>Boolean indicating if the Post Image Entity exists</returns>
        public static bool HasPostImage(this IPluginExecutionContext context, string name)
        {
            return context.PostEntityImages.ContainsKey(name);
        }

        /// <summary>
        /// Checks if there is a Post Image Entity matching the name "PostImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Boolean indicating if the Post Image Entity exists</returns>
        public static bool HasPostImage(this IPluginExecutionContext context)
        {
            return context.PostEntityImages.ContainsKey(ImageNames.PostImageName);
        }

        /// <summary>
        /// WARNING: this method should be avoided and the GetPostImage() method used instead.
        /// Gets the Post Image with the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Post Image Entity name</param>
        /// <returns>The Post Image Entity</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PostImage</exception>
        public static Entity GetPostImage(this IPluginExecutionContext context, string name, ITracingService tracer = null)
        {
            if (!context.HasPostImage(name))
            {
                throw new ArgumentException($"Post Image with name \"{name}\" does not exist.");
            }

            var image = (Entity)context.PostEntityImages[name];

            tracer?.TraceEntity(image, name);

            return image;
        }

        /// <summary>
        /// Gets the Post Image with the name "PostImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The Post Image Entity</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PostImage</exception>
        public static Entity GetPostImage(this IPluginExecutionContext context, ITracingService tracer = null)
        {
            return context.GetPostImage(ImageNames.PostImageName, tracer);
        }

        #endregion
    }
}
