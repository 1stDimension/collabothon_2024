import React from 'react';
import Image from 'next/image';

interface CreditCard {
  type: string;
  lastFourDigits: string;
  owner: string;
  lastTransactionDate: string;
  lastTransactionAmount: string;
  currency: string;
}

const creditCards: CreditCard[] = [
  { type: 'mastercard', lastFourDigits: '3783', owner: 'Wade Warren', lastTransactionDate: '19 Oct, 2024, 23:09', lastTransactionAmount: '-219.78', currency: '€' },
  { type: 'visa', lastFourDigits: '3783', owner: 'Wade Warren', lastTransactionDate: '19 Oct, 2024, 23:09', lastTransactionAmount: '-219.78', currency: '$' },
  { type: 'mastercard', lastFourDigits: '1234', owner: 'John Doe', lastTransactionDate: '18 Oct, 2024, 14:22', lastTransactionAmount: '-150.00', currency: '€' },
  { type: 'visa', lastFourDigits: '5678', owner: 'Jane Smith', lastTransactionDate: '17 Oct, 2024, 09:45', lastTransactionAmount: '-89.99', currency: '$' },
];

const CreditCards: React.FC = () => (
  <div className="bg-white shadow-sm rounded-lg p-6">
    <div className="flex justify-between items-center mb-4">
      <h2 className="text-lg font-bold">Credit cards <span className="bg-yellow-400 text-white text-xs rounded-full px-2 py-1">65</span></h2>
      <p className="text-sm text-gray-500">Last used</p>
    </div>
    <table className="w-full text-left">
      <tbody>
        {creditCards.map((card, index) => (
          <tr key={index} className={index === creditCards.length - 1 ? '' : 'border-b'}>
            <td className="py-2 flex items-center">
              <Image src={card.type === 'visa' ? '/image-visa.png' : '/image-mastercard.svg'} alt={card.type} width={40} height={40} />
              <p className="ml-4">** {card.lastFourDigits}</p>
            </td>
            <td className="py-2">{card.owner}</td>
            <td className="py-2">{card.lastTransactionDate}</td>
            <td className="py-2 text-right">{card.lastTransactionAmount}{card.currency}</td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
);

export default CreditCards;

