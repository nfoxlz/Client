using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Compete.Mis.MisControls
{
    internal static class PrintHelper
    {
        public delegate void LoadXpsMethod(DocumentViewer viewer, FlowDocument document);

        //public static void LoadXps(DocumentViewer viewer, FlowDocument document)
        //{
        //    var documentUri = new Uri("pack://InMemoryDocument.xps");
        //    using var stream = new MemoryStream();
        //    using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
        //    {
        //        PackageStore.RemovePackage(documentUri);
        //        PackageStore.AddPackage(documentUri, package);

        //        using (var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, documentUri.AbsoluteUri))
        //            try
        //            {

        //                //将flow document写入基于内存的xps document中去
        //                XpsDocument.CreateXpsDocumentWriter(xpsDocument).Write(((IDocumentPaginatorSource)document).DocumentPaginator);

        //                //获取这个基于内存的xps document的fixed document
        //                //viewer.Document = xpsDocument.GetFixedDocumentSequence();
        //            }
        //            finally
        //            {
        //                //关闭基于内存的xps document
        //                xpsDocument.Close();
        //                package.Close();
        //                //stream.Close();
        //            }
        //    }

        //    stream.Flush();

        //    using (var package = Package.Open(stream, FileMode.Open, FileAccess.Read))
        //    {
        //        PackageStore.RemovePackage(documentUri);
        //        PackageStore.AddPackage(documentUri, package);

        //        using (var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, documentUri.AbsoluteUri))
        //            try
        //            {

        //                //将flow document写入基于内存的xps document中去
        //                //XpsDocument.CreateXpsDocumentWriter(xpsDocument).Write(((IDocumentPaginatorSource)document).DocumentPaginator);

        //                //获取这个基于内存的xps document的fixed document
        //                viewer.Document = xpsDocument.GetFixedDocumentSequence();
        //            }
        //            finally
        //            {
        //                //关闭基于内存的xps document
        //                xpsDocument.Close();
        //                package.Close();
        //                //stream.Close();
        //            }
        //    }

        //    stream.Close();
        //}

        public static void LoadXps(DocumentViewer viewer, FlowDocument document)
        {
            // 创建临时XPS文件路径
           var tempPath = Path.GetTempFileName();
            File.Delete(tempPath); // 确保文件不存在

            // 创建XPS文档
            using var xpsDoc = new XpsDocument(tempPath, FileAccess.ReadWrite);

            // 获取FlowDocument（假设flowDoc是已创建的FlowDocument对象）
            // 将FlowDocument写入XPS文档
            XpsDocument.CreateXpsDocumentWriter(xpsDoc).Write(((IDocumentPaginatorSource)document).DocumentPaginator);

            // 从XPS文档获取固定文档序列
            // 绑定到DocumentViewer
            viewer.Document = xpsDoc.GetFixedDocumentSequence();
        }

        //public static void LoadXps(DocumentViewer viewer, FlowDocument document)
        //{
        //    using MemoryStream stream = new MemoryStream();
        //    Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);// 创建内存中的XPS文档包
        //    var documentUri = new Uri("pack://InMemoryDocument.xps");
        //    PackageStore.RemovePackage(documentUri);
        //    PackageStore.AddPackage(documentUri, package);

        //    using XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed, documentUri.AbsoluteUri);  // 创建内存XPS文档
        //    // 写入FlowDocument到内存XPS
        //    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
        //    writer.Write(((IDocumentPaginatorSource)document).DocumentPaginator);

        //    // 直接从内存获取固定文档序列
        //    var fixedDoc = xpsDoc.GetFixedDocumentSequence();
        //    // 绑定到DocumentViewer
        //    viewer.Document = fixedDoc;
        //}


        public static void LoadManyXps(DocumentViewer viewer, IEnumerable<FlowDocument> documents)
        {
            //------------------定义新文档的结构
            var fixedDocumentSequence = new FixedDocumentSequence();//创建一个新的文档

            var index = 0UL;
            foreach (var document in documents)
            {
                //构造一个基于内存的xps document
                var stream = new MemoryStream();
                var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
                var documentUri = new Uri("pack://InMemoryDocument" + index.ToString() + ".xps");
                PackageStore.RemovePackage(documentUri);
                PackageStore.AddPackage(documentUri, package);
                var xpsDocument = new XpsDocument(package, CompressionOption.Fast, documentUri.AbsoluteUri);

                //将flow document写入基于内存的xps document中去
                var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(((IDocumentPaginatorSource)document).DocumentPaginator);

                var newDocumentReference = AddPage(xpsDocument);//加入第一个文件
                fixedDocumentSequence.References.Add(newDocumentReference);

                //关闭基于内存的xps document
                xpsDocument.Close();
                index++;
            }

            string newFile = "xpsShow.xps";
            File.Delete(newFile);
            //xps写入新文件
            var newXpsDocument = new XpsDocument(newFile, FileAccess.ReadWrite);
            var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(newXpsDocument);
            xpsDocumentWriter.Write(fixedDocumentSequence);

            //获取这个基于内存的xps document的fixed document
            viewer.Document = newXpsDocument.GetFixedDocumentSequence();
            newXpsDocument.Close();
        }

        public static DocumentReference AddPage(XpsDocument xpsDocument)
        {
            var documentReference = new DocumentReference();
            var fixedDocument = new FixedDocument();

            var documentSequence = xpsDocument.GetFixedDocumentSequence();

            foreach (DocumentReference reference in documentSequence.References)
            {
                var document = reference.GetDocument(false);

                foreach (PageContent pageContent in document.Pages)
                {
                    var uSource = pageContent.Source;//读取源地址
                    var uBase = (pageContent as IUriContext).BaseUri;//读取目标页面地址

                    var newPageContent = new PageContent();
                    newPageContent.GetPageRoot(false);
                    newPageContent.Source = uSource;
                    (newPageContent as IUriContext).BaseUri = uBase;
                    fixedDocument.Pages.Add(newPageContent);//将新文档追加到新的documentRefences中
                }
            }
            documentReference.SetDocument(fixedDocument);
            xpsDocument.Close();

            return documentReference;
        }
    }
}
