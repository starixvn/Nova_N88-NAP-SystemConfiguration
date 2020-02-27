namespace Fanex.NAP.Admin.Core.Web
{
    /// <summary>
    /// User agent tools.
    /// <see cref="http://nerds.palmdrive.net/useragent/index.html"/>
    /// </summary>
    public static class UserAgentTools
    {
        public static string[] GetArray(string a, string b)
        {
            string[] res = new string[2];

            res[0] = a;
            res[1] = b;

            return res;
        }

        /// <summary>
        /// Get bot name.
        /// </summary>
        /// <param name="userAgentValue">The user agent information.</param>
        /// <returns>Bot name string array.</returns>
        public static string[] GetBotName(string userAgentValue)
        {
            var userAgent = userAgentValue.ToLowerInvariant();

            if (userAgent.Contains("help.yahoo.com/"))
            {
                return GetArray("Yahoo", "Yahoo");
            }
            else if (userAgent.Contains("google/"))
            {
                return GetArray("Google", "Google");
            }
            else if (userAgent.Contains("msnbot/"))
            {
                return GetArray("MSNBot", "MSNBot");
            }
            else if (userAgent.Contains("googlebot/"))
            {
                return GetArray("Google", "Google");
            }
            else if (userAgent.Contains("webcrawler/"))
            {
                return GetArray("WebCrawler", "WebCrawler");
            }
            else if (userAgent.Contains("inktomi"))
            {
                return GetArray("Inktomi", "Inktomi");
            }
            else if (userAgent.Contains("teoma"))
            {
                return GetArray("Teoma", "Teoma");
            }
            else
            {
                return null;
            }
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high

        /// <summary>
        /// Get operation system name.
        /// </summary>
        /// <param name="userAgent">The user agent information.</param>
        /// <returns>Operation system array string.</returns>
        public static string[] GetOS(string userAgent)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            string[] arrayBog = GetBotName(userAgent);

            if (arrayBog != null)
            {
                return arrayBog;
            }

            if (userAgent.Contains("Windows-NT"))
            {
                return GetArray("Win", "Window NT");
            }

            if (userAgent.Contains("Windows NT"))
            {
                // The different versions of Windows NT are decoded in the verbosity level 2
                // IE: Windows NT 5.1 = Windows XP
                if (userAgent.Contains("Windows NT 5.1"))
                {
                    return GetArray("Win", "Window XP");
                }

                if (userAgent.Contains("Windows NT 6.0"))
                {
                    return GetArray("Win", "Window Vista");
                }

                if (userAgent.Contains("Windows NT 6.1"))
                {
                    return GetArray("Win", "Window 7");
                }

                if (userAgent.Contains("Windows NT 5.0"))
                {
                    return GetArray("Win", "Window 2000");
                }

                if (userAgent.Contains("Windows NT 5.2"))
                {
                    return GetArray("Win", "Window 2003");
                }

                if (userAgent.Contains("Windows NT 4.0"))
                {
                    return GetArray("Win", "Windows NT4");
                }

                if (userAgent.Contains("Windows NT)"))
                {
                    return GetArray("Win", "Windows NT");
                }

                if (userAgent.Contains("Windows NT;"))
                {
                    return GetArray("Win", "Windows NT");
                }
            }

            if (userAgent.Contains("Win") && userAgent.Contains("Windows"))
            {
                if (userAgent.Contains("Windows 98"))
                {
                    return GetArray("Win", "Windows 98");
                }

                if (userAgent.Contains("Windows_98"))
                {
                    return GetArray("Win", "Windows 98");
                }

                if (userAgent.Contains("Windows 2000"))
                {
                    return GetArray("Win", "Window 2000");
                }

                if (userAgent.Contains("Windows 95"))
                {
                    return GetArray("Win", "Window 95");
                }

                if (userAgent.Contains("Windows 9x"))
                {
                    return GetArray("Win", "Window 9x");
                }

                if (userAgent.Contains("Windows ME"))
                {
                    return GetArray("Win", "Window ME");
                }

                if (userAgent.Contains("Windows CE;"))
                {
                    return GetArray("Win", "Window CE");
                }

                if (userAgent.Contains("Windows 3.1"))
                {
                    return GetArray("Win", "Window 3.1");
                }
                //// If no version was found, rely on the following code to detect "WinXX"
                //// As some User-Agents include two references to Windows
                //// Ex: Mozilla/5.0 (Windows; U; Win98; en-US; rv:1.5)
            }

            if (userAgent.Contains("Mac OS X"))
            {
                if (userAgent.Contains("iPhone"))
                {
                    return userAgent.Contains("iPod") ? GetArray("iOS", "iOS-iPod") : GetArray("iOS", "iOS-iPhone");
                }

                return userAgent.Contains("iPad") ?
                    GetArray("iOS", "iOS-iPad") :
                    GetArray("Mac", "MacOSX");
            }

            if (userAgent.Contains("Android"))
            {
                return GetArray("Linux", "Android");
            }

            if (userAgent.Contains("Mac_PowerPC"))
            {
                return GetArray("Mac", "MacPPC");
            }

            if (userAgent.Contains("Macintosh"))
            {
                return userAgent.Contains("PPC") ? GetArray("Mac", "MacPPC") : GetArray("Mac?", "Mac?");
            }

            if (userAgent.Contains("FreeBSD"))
            {
                return GetArray("*BSD", "*BSD FreeBSD");
            }

            if (userAgent.Contains("OpenBSD"))
            {
                return GetArray("*BSD", "*BSD OpenBSD");
            }

            if (userAgent.Contains("Linux"))
            {
                if (userAgent.Contains("Ubuntu/"))
                {
                    return GetArray("Linux", "Ubuntu");
                }

                return GetArray("Linux", "Linux");
            }

            if (userAgent.Contains("CentOS"))
            {
                return GetArray("Linux", "Linux CentOS");
            }

            if (userAgent.Contains("NetBSD"))
            {
                return GetArray("*BSD", "*BSD NetBSD");
            }

            if (userAgent.Contains("Unix"))
            {
                return GetArray("Linux", "Linux");
            }

            if (userAgent.Contains("SunOS"))
            {
                return GetArray("Unix", "SunOS");
            }

            if (userAgent.Contains("IRIX"))
            {
                return GetArray("Unix", "IRIX");
            }

            if (userAgent.Contains("SonyEricsson"))
            {
                return GetArray("SonyEricsson", "SonyEricsson");
            }

            if (userAgent.Contains("Nokia"))
            {
                return GetArray("Nokia", "Nokia");
            }

            if (userAgent.Contains("BlackBerry"))
            {
                return GetArray("BlackBerry", "BlackBerry");
            }

            if (userAgent.Contains("SymbianOS"))
            {
                return GetArray("SymbianOS", "SymbianOS");
            }

            if (userAgent.Contains("BeOS"))
            {
                return GetArray("BeOS", "BeOS");
            }

            if (userAgent.Contains("Nintendo Wii"))
            {
                return GetArray("Nintendo Wii", "Nintendo Wii");
            }

            if (userAgent.Contains("J2ME/MIDP"))
            {
                return GetArray("Java", "J2ME");
            }

            return GetArray("<b>?</b>", "<b>?</b>");
        }
    }
}