package com.example.psu_mobile

import com.google.zxing.qrcode.encoder.QRCode
import com.squareup.moshi.*
import kotlin.String

class UserInfo {
    var Login : String = "Admin"
    var PassHash : String = "Admin"
    var Name : String = ""
    var Surname : String = ""
    var Futhname : String = ""
    var Group : String = ""
    var Latitude : Double = 0.0
    var Longitude : Double = 0.0
    var Qrqode : QRCode? = null
}