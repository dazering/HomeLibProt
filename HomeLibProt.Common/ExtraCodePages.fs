module HomeLibProt.Common.ExtraCodePages

let registerExtraCodePages () =
    System.Text.Encoding.RegisterProvider System.Text.CodePagesEncodingProvider.Instance
