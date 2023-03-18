const sgMail = require('@sendgrid/mail')

sgMail.setApiKey(process.env.EnigmaDragonsSendGridApiKey)

const msg = {
  from: 'games@enigmadragons.com',
  //template_id: 'd-396a90e0724641d1a1ab97cbf1c5d3be',
  template_id: 'd-94110bbc624747ca91e1eb70c1f3178b',
  personalizations: [{
    to: { email: 'splattercatgaming@gmail.com' },
    dynamic_template_data: { 
      "name": "Splatter Cat",
      "shortGigDesc": "cover our game",
      "longerGigDesc": "create a video featuring Metroplex Zero",
      "priceUsd": "(name your price)",
      "steamKey": "XPBIM-0L0Z9-VQIYE",
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
