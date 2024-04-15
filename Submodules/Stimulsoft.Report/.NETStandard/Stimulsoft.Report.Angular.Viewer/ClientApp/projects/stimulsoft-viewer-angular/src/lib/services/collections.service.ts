import { Injectable } from '@angular/core';
import { ModelService } from './model.service';
import { Item } from './objects';

@Injectable()
export class CollectionsService {

  constructor(public model: ModelService) { }

  public getImageTypesItems() {
    const items = [];
    if (this.model.options.exports.showExportToImageBmp) { items.push(new Item('Bmp', 'Bmp', null, 'Bmp')); }
    if (this.model.options.exports.showExportToImageGif) { items.push(new Item('Gif', 'Gif', null, 'Gif')); }
    if (this.model.options.exports.showExportToImageJpeg) { items.push(new Item('Jpeg', 'Jpeg', null, 'Jpeg')); }
    if (this.model.options.exports.showExportToImagePcx) { items.push(new Item('Pcx', 'Pcx', null, 'Pcx')); }
    if (this.model.options.exports.showExportToImagePng) { items.push(new Item('Png', 'Png', null, 'Png')); }
    if (this.model.options.exports.showExportToImageTiff) { items.push(new Item('Tiff', 'Tiff', null, 'Tiff')); }
    if (this.model.options.exports.showExportToImageMetafile) { items.push(new Item('Emf', 'Emf', null, 'Emf')); }
    if (this.model.options.exports.showExportToImageSvg) { items.push(new Item('Svg', 'Svg', null, 'Svg')); }
    if (this.model.options.exports.showExportToImageSvgz) { items.push(new Item('Svgz', 'Svgz', null, 'Svgz')); }

    return items;
  }

  public getDataTypesItems() {
    const items = [];
    if (this.model.options.exports.showExportToCsv) { items.push(new Item('Csv', 'Csv', null, 'Csv')); }
    if (this.model.options.exports.showExportToDbf) { items.push(new Item('Dbf', 'Dbf', null, 'Dbf')); }
    if (this.model.options.exports.showExportToXml) { items.push(new Item('Xml', 'Xml', null, 'Xml')); }
    if (this.model.options.exports.showExportToDif) { items.push(new Item('Dif', 'Dif', null, 'Dif')); }
    if (this.model.options.exports.showExportToSylk) { items.push(new Item('Sylk', 'Sylk', null, 'Sylk')); }
    if (this.model.options.exports.showExportToJson) { items.push(new Item('Json', 'Json', null, 'Json')); }

    return items;
  }

  public getExcelTypesItems() {
    const items = [];
    if (this.model.options.exports.showExportToExcel2007) { items.push(new Item('Excel2007', 'Excel', null, 'Excel2007')); }
    if (this.model.options.exports.showExportToExcel) { items.push(new Item('ExcelBinary', 'Excel 97-2003', null, 'ExcelBinary')); }
    if (this.model.options.exports.showExportToExcelXml) { items.push(new Item('ExcelXml', 'Excel Xml 2003', null, 'ExcelXml')); }

    return items;
  }

  public getHtmlTypesItems() {
    const items = [];
    if (this.model.options.exports.showExportToHtml) { items.push(new Item('Html', 'Html', null, 'Html')); }
    if (this.model.options.exports.showExportToHtml5) { items.push(new Item('Html5', 'Html5', null, 'Html5')); }
    if (this.model.options.exports.showExportToMht) { items.push(new Item('Mht', 'Mht', null, 'Mht')); }

    return items;
  }

  public getZoomItems() {
    const items = [];
    [0.25, 0.5, 0.75, 1, 1.25, 1.5, 2].forEach((item, i) => items.push(new Item('item' + i, (item * 100) + '%', null, item)));
    return items;
  }

  public getImageFormatForHtmlItems() {
    const items = [];
    items.push(new Item('item0', 'Jpeg', null, 'Jpeg'));
    items.push(new Item('item1', 'Gif', null, 'Gif'));
    items.push(new Item('item2', 'Bmp', null, 'Bmp'));
    items.push(new Item('item3', 'Png', null, 'Png'));

    return items;
  }

  public getExportModeItems() {
    const items = [];
    items.push(new Item('item0', 'Table', null, 'Table'));
    items.push(new Item('item1', 'Span', null, 'Span'));
    items.push(new Item('item2', 'Div', null, 'Div'));

    return items;
  }

  public getImageResolutionItems() {
    const items = [];
    ['10', '25', '50', '75', '100', '200', '300', '400', '500'].forEach((item, i) => items.push(new Item('item' + i, item, null, parseInt(item, 10))));
    return items;
  }

  public getImageCompressionMethodItems() {
    const items = [];
    items.push(new Item('item0', 'Jpeg', null, 'Jpeg'));
    items.push(new Item('item1', 'Flate', null, 'Flate'));
    return items;
  }

  public getImageQualityItems() {
    const items = [];
    [0.25, 0.5, 0.75, 0.85, 0.9, 0.95, 1].forEach((item, i) => items.push(new Item('item' + i, (item * 100).toString(), null, item)));
    return items;
  }

  public getBorderTypeItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('BorderTypeSimple'), null, 'Simple'));
    items.push(new Item('item1', this.model.loc('BorderTypeSingle'), null, 'UnicodeSingle'));
    items.push(new Item('item2', this.model.loc('BorderTypeDouble'), null, 'UnicodeDouble'));

    return items;
  }

  public getEncodingDataItems() {
    const items = [];
    this.model.encodingData.forEach((item, i) => items.push(new Item('item' + i, item.value, null, item.key)));
    return items;
  }

  public getImageFormatItems(withoutMonochrome: boolean = false) {
    const items = [];
    items.push(new Item('item0', this.model.loc('ImageFormatColor'), null, 'Color'));
    items.push(new Item('item1', this.model.loc('ImageFormatGrayscale'), null, 'Grayscale'));
    if (!withoutMonochrome) { items.push(new Item('item2', this.model.loc('ImageFormatMonochrome'), null, 'Monochrome')); }

    return items;
  }

  public getMonochromeDitheringTypeItems() {
    const items = [];
    items.push(new Item('item0', 'None', null, 'None'));
    items.push(new Item('item1', 'FloydSteinberg', null, 'FloydSteinberg'));
    items.push(new Item('item2', 'Ordered', null, 'Ordered'));

    return items;
  }

  public getTiffCompressionSchemeItems() {
    const items = [];
    items.push(new Item('item0', 'Default', null, 'Default'));
    items.push(new Item('item1', 'CCITT3', null, 'CCITT3'));
    items.push(new Item('item2', 'CCITT4', null, 'CCITT4'));
    items.push(new Item('item3', 'LZW', null, 'LZW'));
    items.push(new Item('item4', 'None', null, 'None'));
    items.push(new Item('item5', 'Rle', null, 'Rle'));

    return items;
  }

  public getEncodingDifFileItems() {
    const items = [];
    items.push(new Item('item0', '437', null, '437'));
    items.push(new Item('item1', '850', null, '850'));
    items.push(new Item('item2', '852', null, '852'));
    items.push(new Item('item3', '857', null, '857'));
    items.push(new Item('item4', '860', null, '860'));
    items.push(new Item('item5', '861', null, '861'));
    items.push(new Item('item6', '862', null, '862'));
    items.push(new Item('item7', '863', null, '863'));
    items.push(new Item('item8', '865', null, '865'));
    items.push(new Item('item9', '866', null, '866'));
    items.push(new Item('item10', '869', null, '869'));

    return items;
  }

  public getExportModeRtfItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('ExportModeRtfTable'), null, 'Table'));
    items.push(new Item('item1', this.model.loc('ExportModeRtfFrame'), null, 'Frame'));

    return items;
  }

  public getEncodingDbfFileItems() {
    const items = [];
    items.push(new Item('item0', 'Default', null, 'Default'));
    items.push(new Item('item1', '437 U.S. MS-DOS', null, 'USDOS'));
    items.push(new Item('item2', '620 Mazovia(Polish) MS-DOS', null, 'MazoviaDOS'));
    items.push(new Item('item3', '737 Greek MS-DOS(437G)', null, 'GreekDOS'));
    items.push(new Item('item4', '850 International MS-DOS', null, 'InternationalDOS'));
    items.push(new Item('item5', '852 Eastern European MS-DOS', null, 'EasternEuropeanDOS'));
    items.push(new Item('item6', '857 Turkish MS-DOS', null, 'TurkishDOS'));
    items.push(new Item('item7', '861 Icelandic MS-DOS', null, 'IcelandicDOS'));
    items.push(new Item('item8', '865 Nordic MS-DOS', null, 'NordicDOS'));
    items.push(new Item('item9', '866 Russian MS-DOS', null, 'RussianDOS'));
    items.push(new Item('item10', '895 Kamenicky(Czech) MS-DOS', null, 'KamenickyDOS'));
    items.push(new Item('item11', '1250 Eastern European Windows', null, 'EasternEuropeanWindows'));
    items.push(new Item('item12', '1251 Russian Windows', null, 'RussianWindows'));
    items.push(new Item('item13', '1252 WindowsANSI', null, 'WindowsANSI'));
    items.push(new Item('item14', '1253 GreekWindows', null, 'GreekWindows'));
    items.push(new Item('item15', '1254 TurkishWindows', null, 'TurkishWindows'));
    items.push(new Item('item16', '10000 StandardMacintosh', null, 'StandardMacintosh'));
    items.push(new Item('item17', '10006 GreekMacintosh', null, 'GreekMacintosh'));
    items.push(new Item('item18', '10007 RussianMacintosh', null, 'RussianMacintosh'));
    items.push(new Item('item19', '10029 EasternEuropeanMacintosh', null, 'EasternEuropeanMacintosh'));

    return items;
  }

  public getAllowEditableItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('NameYes'), null, 'Yes'));
    items.push(new Item('item1', this.model.loc('NameNo'), null, 'No'));

    return items;
  }

  public getEncryptionKeyLengthItems() {
    const items = [];
    items.push(new Item('item0', '40 bit RC4 (Acrobat 3)', null, 'Bit40'));
    items.push(new Item('item1', '128 bit RC4 (Acrobat 5)', null, 'Bit128'));
    items.push(new Item('item2', '128 bit AES (Acrobat 7)', null, 'Bit128_r4'));
    items.push(new Item('item3', '256 bit AES (Acrobat 9)', null, 'Bit256_r5'));
    items.push(new Item('item4', '256 bit AES (Acrobat X)', null, 'Bit256_r6'));

    return items;
  }

  public getDataExportModeItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('BandsFilterDataOnly'), null, 'Data'));
    items.push(new Item('item1', this.model.loc('BandsFilterDataAndHeaders'), null, 'DataAndHeaders'));
    items.push(new Item('item2', this.model.loc('BandsFilterDataAndHeadersFooters'), null, 'DataAndHeadersFooters'));
    items.push(new Item('item3', this.model.loc('BandsFilterAllBands'), null, 'AllBands'));
    return items;
  }

  public getFilterConditionItems(dataType) {
    const items = [];
    switch (dataType) {
      case 'String':
        items.push(new Item('item0', this.model.loc('ConditionEqualTo'), '', 'EqualTo'));
        items.push(new Item('item1', this.model.loc('ConditionNotEqualTo'), '', 'NotEqualTo'));
        items.push('separator1');
        items.push(new Item('item2', this.model.loc('ConditionContaining'), '', 'Containing'));
        items.push(new Item('item3', this.model.loc('ConditionNotContaining'), '', 'NotContaining'));
        items.push('separator2');
        items.push(new Item('item4', this.model.loc('ConditionBeginningWith'), '', 'BeginningWith'));
        items.push(new Item('item5', this.model.loc('ConditionEndingWith'), '', 'EndingWith'));
        items.push('separator3');
        items.push(new Item('item2', this.model.loc('ConditionBetween'), '', 'Between'));
        items.push(new Item('item3', this.model.loc('ConditionNotBetween'), '', 'NotBetween'));
        items.push('separator4');
        items.push(new Item('item6', this.model.loc('ConditionGreaterThan'), '', 'GreaterThan'));
        items.push(new Item('item7', this.model.loc('ConditionGreaterThanOrEqualTo'), '', 'GreaterThanOrEqualTo'));
        items.push('separator5');
        items.push(new Item('item8', this.model.loc('ConditionLessThan'), '', 'LessThan'));
        items.push(new Item('item9', this.model.loc('ConditionLessThanOrEqualTo'), '', 'LessThanOrEqualTo'));
        items.push('separator6');
        items.push(new Item('item10', this.model.loc('ConditionIsNull'), '', 'IsNull'));
        items.push(new Item('item11', this.model.loc('ConditionIsNotNull'), '', 'IsNotNull'));
        items.push('separator7');
        items.push(new Item('item12', this.model.loc('ConditionIsBlank'), '', 'IsBlank'));
        items.push(new Item('item13', this.model.loc('ConditionIsNotBlank'), '', 'IsNotBlank'));
        break;
      case 'Numeric':
      case 'DateTime':
        {
          items.push(new Item('item0', this.model.loc('ConditionEqualTo'), '', 'EqualTo'));
        }
        items.push(new Item('item1', this.model.loc('ConditionNotEqualTo'), '', 'NotEqualTo'));
        items.push('separator1');
        items.push(new Item('item2', this.model.loc('ConditionBetween'), '', 'Between'));
        items.push(new Item('item3', this.model.loc('ConditionNotBetween'), '', 'NotBetween'));
        items.push('separator2');
        items.push(new Item('item4', this.model.loc('ConditionGreaterThan'), '', 'GreaterThan'));
        items.push(new Item('item5', this.model.loc('ConditionGreaterThanOrEqualTo'), '', 'GreaterThanOrEqualTo'));
        items.push('separator3');
        items.push(new Item('item6', this.model.loc('ConditionLessThan'), '', 'LessThan'));
        items.push(new Item('item7', this.model.loc('ConditionLessThanOrEqualTo'), '', 'LessThanOrEqualTo'));
        items.push('separator4');
        items.push(new Item('item8', this.model.loc('ConditionIsNull'), '', 'IsNull'));
        items.push(new Item('item9', this.model.loc('ConditionIsNotNull'), '', 'IsNotNull'));
        break;
      case 'Boolean':
        {
          items.push(new Item('item0', this.model.loc('ConditionEqualTo'), '', 'EqualTo'));
        }
        items.push(new Item('item1', this.model.loc('ConditionNotEqualTo'), '', 'NotEqualTo'));
        break;
    }

    return items;
  }

  public getBoolItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('NameTrue'), null, 'True'));
    items.push(new Item('item1', this.model.loc('NameFalse'), null, 'False'));

    return items;
  }

  public getPaperSizesItems() {
    const items = [];
    this.model.paperSizes.forEach((item, i) => items.push(new Item('item' + i, item, null, item)));
    return items;
  }

  public getOrientationItems() {
    const items = [];
    items.push(new Item('item0', this.model.loc('Portrait'), null, 'Portrait'));
    items.push(new Item('item1', this.model.loc('Landscape'), null, 'Landscape'));

    return items;
  }

  public getDashboardImageQualityItems() {
    const items = [];
    [50, 75, 100, 150, 200, 300, 500].forEach((item, i) => items.push(new Item('item' + i, item + '%', null, item.toString())));
    return items;
  }

  public getPdfSecurityCertificatesItems() {
    let items = [];
    if (this.model.pdfSecurityCertificates) {
      for (var i = 0; i < this.model.pdfSecurityCertificates.length; i++) {
        var item = this.model.pdfSecurityCertificates[i];
        items.push(new Item('item' + i, "Name: " + item.name + "<br>Issuer: " + item.issuer + "<br>Valid from: " + item.from + " to " + item.to, null, item.thumbprint, null, null, null, 'DigitalSignature'));
      }
    }
    return items;
  }

}
