package com.example.psu_mobile

import android.net.Uri
import com.google.zxing.qrcode.encoder.QRCode
import com.squareup.moshi.*
import kotlin.String

class UserInfo {
    var Login : String = "Admin"
    var Password : String = "Admin"
    var passHash : String = "Admin"
    var Name : String = "Александр"
    var Surname : String = "Пушкин"
    var Midname : String = "Сергеевич"
    var Group : String = "ЛИТ-1"
    var Latitude : Double = 0.0
    var Longitude : Double = 0.0
    var Qrqode : String = ""
    var userPhoto: String = ""
}