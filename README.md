PrintPDF
=========================

Conversion en fichier pdf de page HTML.
Ce projet a originellement était développé pour [Éduthèque](https://edutheque.philharmoniedeparis.fr/) par Cécile Briard (cebriard.ccb[at]gmail[point]com).

## Librairies externes utilisées
- L'outil open source [wkhtmltopdf](https://wkhtmltopdf.org/) est utilisé pour générer le pdf à partir du HTML.
- wkhtmltopdf est appelé avec le wrapper C# [Codaxy.WkHtmlToPdf](https://github.com/codaxy/wkhtmltopdf). C'est le fichier `PdfConvert.cs`.
- [Html Agility Pack](https://html-agility-pack.net/) est utilisé pour modifier la structure du HTML avant sa conversion en pdf.

## Organisation du code
- le fichier principal de l'application est `Common.cs`.
- `QRCode.ashx` permet de générer un QRCode qui est ajouté au pdf.
- la feuille de style `PDF-print.css` modifie l'affichage des éléments HTML afin d'obtenir une version plus print-fiendly : suppression des applats de couleur, masquage des éléments inutiles, etc.

## Installation

Prérequis : avoir installé le binaire de [wkhtmltopdf](https://wkhtmltopdf.org/) sur sa machine

```
# Cloner ce répertoire
git clone https://github.com/philharmoniedeparis/PrintPDF
```

## Licence

© Cité de la musique - Philharmonie de Paris, 2019
