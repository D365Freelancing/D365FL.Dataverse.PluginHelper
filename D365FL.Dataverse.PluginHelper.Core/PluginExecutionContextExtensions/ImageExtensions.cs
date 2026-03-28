using Microsoft.Xrm.Sdk;
using System;
using System.Xml.Linq;


namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct ImageNames
    {
        internal const string PreImageName = "PreImage";
        internal const string PostImageName = "PostImage";
    }

    public static class ImageExtensions
    {
        #region "Pre Image"
        /// <summary>
        /// WARNING: this method should be avoided and the HasPreImage() method used instead.
        /// Checks if there is a Pre Image Entity mathcing the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Pre Image Entity name</param>
        /// <returns>Boolean indicating if the Pre Image Entity exists</returns>
        public static bool HasPreImage(this IPluginExecutionContext context, string name)
        {
            return context.PreEntityImages.ContainsKey(name);
        }

        /// <summary>
        /// Checks if there is a Pre Image Entity mathcing the name "PreImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Boolean indicating if the Pre Image Entity exists</returns>
        public static bool HasPreImage(this IPluginExecutionContext context)
        {
            return context.PreEntityImages.ContainsKey(ImageNames.PreImageName);
        }

        /// <summary>
        /// WARNING: this method should be avoided and the GetPreImage() method used instead.
        /// Gets the Pre Image with the inputted named
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Pre Image Entity name</param>
        /// <returns>The Pre Image Entiy</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PreImage</exception>
        public static Entity GetPreImage(this IPluginExecutionContext context, string name)
        {
            if (!context.HasPreImage(name))
            {
                throw new ArgumentException($"Pre Image with name \"{name}\" does not exist.");
            }
            return (Entity)context.PreEntityImages[name];
        }

        /// <summary>
        /// Gets the Pre Image with the named "PreImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The Pre Image Entiy</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PreImage</exception>
        public static Entity GetPreImage(this IPluginExecutionContext context)
        {
            return context.GetPreImage(ImageNames.PreImageName);
        }

        #endregion

        #region "Post Image"
        /// <summary>
        /// WARNING: this method should be avoided and the HasPostImage() method used instead.
        /// Checks if there is a Post Image Entity mathcing the inputted name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Post Image Entity name</param>
        /// <returns>Boolean indicating if the Post Image Entity exists</returns>
        public static bool HasPostImage(this IPluginExecutionContext context, string name)
        {
            return context.PostEntityImages.ContainsKey(name);
        }

        /// <summary>
        /// Checks if there is a Pst Image Entity mathcing the name "PostImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Boolean indicating if the Post Image Entity exists</returns>
        public static bool HasPostImage(this IPluginExecutionContext context)
        {
            return context.PostEntityImages.ContainsKey(ImageNames.PostImageName);
        }

        /// <summary>
        /// WARNING: this method should be avoid and the GetPostImage() method used instead.
        /// Gets the Post Image with the inputted named
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The Post Image Entity name</param>
        /// <returns>The Post Image Entiy</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PostImage</exception>
        public static Entity GetPostImage(this IPluginExecutionContext context, string name)
        {
            if (!context.HasPostImage(name))
            {
                throw new ArgumentException($"Post Image with name \"{name}\" does not exist.");
            }
            return (Entity)context.PostEntityImages[name];
        }

        /// <summary>
        /// Gets the Post Image with the named "PostImage"
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The Post Image Entiy</returns>
        /// <exception cref="ArgumentException">Exception is thrown if inputted name does not match a PostImage</exception>
        public static Entity GetPostImage(this IPluginExecutionContext context)
        {
            return context.GetPostImage(ImageNames.PostImageName);
        }

        #endregion
    }
}
