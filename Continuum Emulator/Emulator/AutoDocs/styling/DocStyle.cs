namespace Continuum93.Emulator.AutoDocs.styling
{
    /// <summary>
    /// Provides static methods to retrieve CSS stylesheets used for auto-generated documentation HTML.
    /// </summary>
    public static class DocStyle
    {
        /// <summary>
        /// Returns the default CSS stylesheet primarily used for displaying assembly reference data,
        /// including detailed register/memory bit layouts.
        /// </summary>
        public static string GetDefaultStyle()
        {
            return @"/* Stylesheet for the assembly reference file */

table, th, td {
	text-align: left;
    border:1px solid #ddd;
    padding:0px;
    vertical-align:top;
}

table {
	table-layout: fixed;
}

.bits
{
    font-family:courier, courier new, serif;
    font-size: 1em;
    font-weight:999;
}

.grayBit {
	color: #888;
    background-color:black;
    border-left: 1px solid #888;
    border-right: 1px solid #888;
}

.unused {
	font-style: italic;
}

.emphasizeMiddleLeft {
	border-left: 1px solid black;
}

.emphasizeMiddleRight {
	border-right: 1px solid black;
}

.emphasizeBottom {
	border: 1px solid black;
    border-top: 0px;
}

.intTable {
	padding: 4px;
}

#subscript {  
	vertical-align:sub;
	font-size: small;
}

pre {
    font-family: 'Courier New', Courier, monospace;
    padding: 1em;
    background-color: #dedede;
    margin-left: 1em;
}";
        }

        /// <summary>
        /// Returns the CSS stylesheet used for supplementary information files, 
        /// such as instruction descriptions and general documentation.
        /// </summary>
        public static string GetInfoStyle()
        {
            return @"/* Stylesheet for the assembly reference info file */

td {
    border-top: 1px solid #ddd;
}

table, th, td {
	text-align: left;
    
    padding:0;
    vertical-align:top;
}

table {
	table-layout: fixed;
}

.first-column {
    min-width: 200px;
    width: 200px;
    
}

.second-column {

}

pre {
    font-family: 'Courier New', Courier, monospace;
    padding: 1em;
    background-color: #dedede;
    margin-left: 1em;
}";
        }
    }
}
