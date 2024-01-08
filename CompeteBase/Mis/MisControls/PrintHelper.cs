using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;

namespace Compete.Mis.MisControls
{
    internal static class PrintHelper
    {
        public delegate void LoadXpsMethod(DocumentViewer viewer, FlowDocument document);

        public static void LoadXps(DocumentViewer viewer, FlowDocument document)
        {
            using var stream = new MemoryStream();
            var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            var DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);

            var xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            try
            {

                //将flow document写入基于内存的xps document中去
                var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(((IDocumentPaginatorSource)document).DocumentPaginator);

                //获取这个基于内存的xps document的fixed document
                viewer.Document = xpsDocument.GetFixedDocumentSequence();
            }
            finally
            {
                //关闭基于内存的xps document
                xpsDocument.Close();
            }
        }

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
