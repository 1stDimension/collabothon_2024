'use client';

import React, { useState, useEffect } from 'react';
import Image from 'next/image';

export default function CurrentDate() {
  const [currentDate, setCurrentDate] = useState(new Date().toLocaleString('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }));

  useEffect(() => {
    const intervalId = setInterval(() => {
      setCurrentDate(new Date().toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }));
    }, 60000);

    return () => clearInterval(intervalId);
  }, []);

  return (
    <div className="flex items-center bg-[#E0E6E9] text-white w-[178px] h-[32px] px-4 py-2 rounded-[8px]">
      <Image src="/icon-clock.svg" alt="Clock Icon" width={15} height={15} />
      <span className="ml-2 font-bold text-[15px] text-[#002F3F]">{currentDate}</span>
    </div>
  );
}
