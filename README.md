<p align="center"> 
  <img src="readmeresources/fencrypt_new_logo.png" alt="fencrypt logo" width="680px" height="156px">
</p>
<h1 align="center"> fencrypt </h1>
<h3 align="center"> An AES file encryption app built with Xamarin in C# and designed to be compatible on all Android API releases.</h3> 

</br>

<!-- TABLE OF CONTENTS -->
<h2 id="table-of-contents"> :book: Table of Contents</h2>
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project"> ➤ About The Project</a></li>
    <li><a href="#prerequisites"> ➤ Prerequisites</a></li>
    <li><a href="#roadmap"> ➤ Roadmap</a></li>
    <!--<li><a href="#experiments">Experiments</a></li>-->
    <li><a href="#references"> ➤ References</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
<h2 id="about-the-project"> :pencil: About The Project</h2>

<p align="justify"> 
  This project aims to create a cross-API compatible Android application for the encryption and decryption of various file formats utilizing the Advanced Encryption Standard (AES)
  with 256-bit key length and 128-bit initialization vector. It currently utilizes a user-selected password which is then put through a SHA-256 hash function to convert a 
  password into the proper length, as well as an MD5 hash function to generate the initialization vector from the user password. AES Encryption occurs in CBC mode for now. While CBC is not used on the internet often due to lack of authentication, for local files which will not require random access, it is more than secure enough
  and greatly improves performance on lower-end hardware.
  As of Android API 29 (Android 10), there were numerous new changes brought about, including the requirement to use Scoped Storage file access -- this means that apps no longer
  have the same file access privaleges as they historically did. Unfortunately, many of the new methods used for creating and reading files break on lower API levels, so this
  necessitates the creation of separate read/write functions - one for API's 28 or below and one for API's 29 or above. There are also plans in the future to improve the UI
  interface and port support over to iOS.
</p>

<p align="center">
  <img src="readmeresources/CBC_encryption.png" alt="Image describing CBC-mode for AES Encryption" width="50%" height="50%">        
  <!--figcaption>Caption goes here</figcaption-->
</p>
