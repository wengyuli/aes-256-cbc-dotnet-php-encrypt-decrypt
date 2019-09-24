<?php

define('FILE_ENCRYPTION_BLOCKS', 1000);
//echo OPENSSL_ZERO_PADDING. '-';
//echo OPENSSL_RAW_DATA;


$input = '/Users/wengyuli/origin.mp4';
$output = '/Users/wengyuli/origin999.mp4.qs';

$fp = fopen($input, 'r+');
$of = fopen($output, 'wb');

$password = '3sc3RLrpd17';
$iv = chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0);
$method = 'aes-256-cbc';
$key = substr(hash('sha256', $password, true), 0, 32);

fwrite($of, $iv);
while(!feof($fp)){
    $plaintext = fread($fp, 16 * FILE_ENCRYPTION_BLOCKS);
    $ciphertext = openssl_encrypt($plaintext, $method, $key, OPENSSL_RAW_DATA, $iv);
    $iv = substr($ciphertext, 0, 16);
    fwrite($of, $ciphertext);
}

$iv = fread($fp, 16);
while(!feof($fp)){
    $ciphertext = fread($fp, 16 * (FILE_ENCRYPTION_BLOCKS + 1));
    $plaintext = openssl_decrypt($ciphertext, $method, $key, OPENSSL_RAW_DATA, $iv);
    $iv = substr($ciphertext, 0, 16);
    fwrite($of, $plaintext);
}

fclose($fp);
fclose($fp);

echo "done";
