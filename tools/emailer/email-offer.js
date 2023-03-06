const sgMail = require('@sendgrid/mail')

sgMail.setApiKey(process.env.EnigmaDragonsSendGridApiKey)

const msg = {
  from: 'games@enigmadragons.com',
  template_id: 'd-396a90e0724641d1a1ab97cbf1c5d3be',
  personalizations: [{
    to: { email: 'seereaxbiz@gmail.com' },
    dynamic_template_data: { 
      "name": "SeeReax",
      "shortGigDesc": "broadcast a stream",
      "longerGigDesc": "broadcast a dedicated 120-minute stream",
      "priceUsd": "I actually don't know how much to offer for someone of your caliber. How much would you want?",
      "steamKey": "8GTA4-VJ89F-XMHCT",
      "senderName": "Silas Reinagel"
    }
  }],
}

sgMail
  .send(msg)
  .then(() => {
    console.log('Email sent')
  })
  .catch((error) => {
    console.error(error)
  })
