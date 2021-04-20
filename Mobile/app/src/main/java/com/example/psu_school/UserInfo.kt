package com.example.psu_mobile

import com.google.zxing.qrcode.encoder.QRCode
import com.squareup.moshi.*
import kotlin.String

class UserInfo {
    var Login : String = "Admin"
    var PassHash : String = "Admin"
    var Name : String = "Александр"
    var Surname : String = "Пушкин"
    var Futhname : String = "Сергеевич"
    var Group : String = "ЛИТ-1"
    var Latitude : Double = 0.0
    var Longitude : Double = 0.0
    var Qrqode : String = ""
}