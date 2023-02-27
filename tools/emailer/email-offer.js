const sgMail = require('@sendgrid/mail')

sgMail.setApiKey(process.env.EnigmaDragonsSendGridApiKey)

const msg = {
  from: 'games@enigmadragons.com',
  template_id: 'd-396a90e0724641d1a1ab97cbf1c5d3be',
  personalizations: [{
    to: { email: '' },
    dynamic_template_data: { 
      "name": "",
      "shortGigDesc": "broadcast a stream",
      "longerGigDesc": "broadcast a dedicated 120-minute stream",
      "priceUsd": "50",
      "steamKey": "",
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
