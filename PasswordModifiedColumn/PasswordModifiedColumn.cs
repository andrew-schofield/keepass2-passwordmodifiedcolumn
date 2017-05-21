using KeePass.Plugins;
using System.Drawing;
using KeePass.Forms;
using PasswordModifiedColumn;
using KeePass.UI;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using KeePassLib;
using System;
using KeePass.Util.Spr;
using PasswordModifiedColumn.Extensions;
using KeePassLib.Utility;

namespace PasswordModifiedColumn
{
    public sealed class PasswordModifiedColumnExt : Plugin
    {
        private static IPluginHost m_host = null;
        private PasswordModifiedColumnProvider m_prov = null;

        internal static IPluginHost Host
        {
            get { return m_host; }
        }

        public override bool Initialize(IPluginHost host)
        {
            Terminate();

            m_host = host;
            if (m_host == null) { Debug.Assert(false); return false; }

            m_prov = new PasswordModifiedColumnProvider();
            m_host.ColumnProviderPool.Add(m_prov);

            m_host.MainWindow.FileClosed += this.OnFileClosed;

            return true;
        }

        public override void Terminate()
        {
            if (m_host == null) return;

            m_host.MainWindow.FileClosed -= this.OnFileClosed;

            m_host.ColumnProviderPool.Remove(m_prov);
            m_prov = null;

            m_host = null;
        }

        private void OnFileClosed(object sender, FileClosedEventArgs e)
        {
            PasswordModifiedColumnProvider.ClearCache();
        }

        public override string UpdateUrl
        {
            get
            {
                return "https://raw.githubusercontent.com/andrew-schofield/keepass2-passwordmodifiedcolumn/master/VERSION";
            }
        }

        public override Image SmallIcon
        {
            get
            {
                return Resources.hibp.ToBitmap();
            }
        }
    }

    public sealed class PasswordModifiedColumnProvider : ColumnProvider
    {
        private const string PmcpName = "Password Last Modified Time";

        private static object m_oCacheSync = new object();
        private static Dictionary<string, DateTime> m_dCache =
            new Dictionary<string, DateTime>();

        private string[] m_vColNames = new string[] { PmcpName };

        public override string[] ColumnNames
        {
            get { return m_vColNames; }
        }

        public override HorizontalAlignment TextAlign
        {
            get { return HorizontalAlignment.Right; }
        }

        internal static void ClearCache()
        {
            lock (m_oCacheSync)
            {
                m_dCache.Clear();
            }
        }
        
        public override string GetCellData(string strColumnName, PwEntry pe)
        {
            if (strColumnName == null) { Debug.Assert(false); return string.Empty; }
            if (strColumnName != PmcpName) return string.Empty;
            if (pe == null) { Debug.Assert(false); return string.Empty; }

            string strPw = pe.Strings.ReadSafe(PwDefs.PasswordField);


            if (strPw.IndexOf('{') >= 0)
            {
                IPluginHost host = PasswordModifiedColumnExt.Host;
                if (host == null) { Debug.Assert(false); return string.Empty; }

                PwDatabase pd = null;
                try
                {
                    pd = host.MainWindow.DocumentManager.SafeFindContainerOf(pe);
                }
                catch (Exception) { Debug.Assert(false); }

                SprContext ctx = new SprContext(pe, pd, (SprCompileFlags.Deref |
                    SprCompileFlags.TextTransforms), false, false);
                strPw = SprEngine.Compile(strPw, ctx);
            }

            DateTime lastModifiedTime;

            lock (m_oCacheSync)
            {
                if (!m_dCache.TryGetValue(strPw, out lastModifiedTime)) lastModifiedTime = DateTime.MaxValue;
            }

            if (lastModifiedTime == DateTime.MaxValue)
            {
                lastModifiedTime = pe.GetPasswordLastModified();

                lock (m_oCacheSync)
                {
                    m_dCache[strPw] = lastModifiedTime;
                }
            }

            return (TimeUtil.ToDisplayString(lastModifiedTime));
        }
    }
}
